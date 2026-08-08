# 🔄 Request Lifecycle

This guide explains how **CoreSystem.Idempotency** processes every incoming request.

Understanding the request lifecycle helps explain how duplicate requests are prevented while ensuring that business operations execute exactly once.

---

# Overview

Every request passes through the idempotency middleware before reaching the application endpoint.

The middleware coordinates the complete idempotency workflow by:

1. Validating whether idempotency applies.
2. Resolving the idempotency key.
3. Generating a request fingerprint.
4. Querying the configured storage implementation.
5. Executing the request or replaying a stored response.
6. Persisting successful responses for future replay.

---

# Request Flow

```text
                  Incoming Request
                         │
                         ▼
              Is Idempotency Enabled?
                         │
            No ───────────────► Continue Pipeline
                         │
                         Yes
                         ▼
            Is HTTP Method Supported?
                         │
            No ───────────────► Continue Pipeline
                         │
                         Yes
                         ▼
             Resolve Idempotency Key
                         │
            Missing ─────────────► Continue Pipeline
                         │
                         Found
                         ▼
          Generate Request Fingerprint
                         │
                         ▼
          Lookup IIdempotencyStorage
                         │
         ┌───────────────┴───────────────┐
         │                               │
         ▼                               ▼
     Entry Found                   Entry Not Found
         │                               │
         ▼                               ▼
 Compare Fingerprint            Execute Endpoint
         │                               │
         ▼                               ▼
 Fingerprint Match?             Capture Response
         │                               │
   ┌─────┴─────┐                         ▼
   │           │                 Persist Response
   │           │                         │
   ▼           ▼                         ▼
Replay      Throw                 Return Response
Response    Exception
```

---

# Step 1 — Validate the Request

The middleware first determines whether idempotency should be applied.

Validation includes:

- The framework is enabled.
- The HTTP method is configured as idempotent.
- An idempotency key is present.

Requests that do not satisfy these requirements continue through the ASP.NET Core pipeline without idempotency processing.

---

# Step 2 — Generate the Request Fingerprint

A fingerprint uniquely identifies the logical request.

Depending on the configured options, the fingerprint may include:

- HTTP method
- Request path
- Query string
- Request body
- Content-Type
- Selected request headers

See **Fingerprinting** for implementation details.

---

# Step 3 — Query the Storage Layer

The middleware queries the configured `IIdempotencyStorage` implementation using the resolved idempotency key.

The storage implementation determines whether the request has already been processed.

CoreSystem.Idempotency is independent from the underlying persistence technology.

---

# Step 4 — Existing Entry

If an idempotency entry already exists:

1. The stored fingerprint is compared with the incoming request.
2. If the fingerprints match:
   - The stored response is replayed immediately.
3. If the fingerprints differ:
   - An `IdempotencyFingerprintMismatchException` is thrown.

The application endpoint is never executed a second time.

---

# Step 5 — New Request

If no entry exists:

1. The request continues through the ASP.NET Core pipeline.
2. The endpoint executes normally.
3. The generated response is captured.
4. The response and its fingerprint are persisted.
5. The response is returned to the client.

---

# Step 6 — Response Replay

When a duplicate request is detected, the middleware reconstructs the original response.

The following information is restored:

- HTTP status code
- Response headers
- Content-Type
- Response body

The client receives exactly the same response that was produced by the original request.

---

# Error Handling

The middleware may terminate request processing under the following conditions.

| Condition | Result |
|----------|--------|
| Idempotency disabled | Request continues through the pipeline |
| Unsupported HTTP method | Request continues through the pipeline |
| Missing idempotency key | Request continues through the pipeline |
| Fingerprint mismatch | `IdempotencyFingerprintMismatchException` |
| Storage failure | Storage-specific exception |
| Cached response found | Endpoint execution is skipped |

See **Errors** for detailed exception documentation.

---

# Performance Considerations

The middleware is optimized for high-throughput APIs.

For duplicate requests:

- Business logic is skipped.
- Expensive operations are avoided.
- The previously stored response is replayed immediately.

This significantly reduces processing time while preventing duplicate side effects.

---

# Related Documentation

- Architecture
- Configuration
- Fingerprinting
- Response Replay
- Storage Providers
- Observability
- Errors