# 🔄 Request Lifecycle

This guide explains how **CoreSystem.Idempotency** processes every incoming request.

Understanding the request lifecycle helps explain how duplicate requests are prevented while ensuring that business operations execute only once.

---

# Overview

Every request passes through the idempotency middleware before reaching the application endpoint.

The middleware performs the following high-level steps:

1. Validate the request
2. Generate a request fingerprint
3. Query the configured storage provider
4. Execute the request or replay a stored response
5. Persist the response
6. Return the response to the client

---

# Request Flow

```text
                  Incoming Request
                         │
                         ▼
          Validate Idempotency Header
                         │
            Missing? ─── Yes ─────► Continue Pipeline
                         │
                         No
                         ▼
          Is HTTP Method Supported?
                         │
            No ───────────────► Continue Pipeline
                         │
                         Yes
                         ▼
          Generate Request Fingerprint
                         │
                         ▼
        Lookup Storage Provider
                         │
         ┌───────────────┴───────────────┐
         │                               │
         ▼                               ▼
   Record Found                   Record Not Found
         │                               │
         ▼                               ▼
 Validate Fingerprint          Execute Endpoint
         │                               │
         ▼                               ▼
 Fingerprint Match?              Capture Response
         │                               │
   ┌─────┴─────┐                         ▼
   │           │                  Persist Response
   │           │                         │
   ▼           ▼                         ▼
Replay      Throw                 Return Response
Response    Exception
```

---

# Step 1 — Validate the Request

The middleware first determines whether idempotency should be applied.

Validation includes:

- The idempotency feature is enabled.
- The HTTP method is configured as idempotent.
- The request contains the configured idempotency header.

Requests that do not satisfy these requirements continue through the ASP.NET Core pipeline without idempotency processing.

---

# Step 2 — Generate the Request Fingerprint

A fingerprint uniquely identifies the logical request.

The fingerprint is generated using the configured request components, such as:

- HTTP method
- Request path
- Query string
- Request body
- Selected headers

See **Fingerprinting** for details.

---

# Step 3 — Query the Storage Provider

The middleware queries the configured storage provider using the idempotency key.

Supported providers include:

- Redis
- PostgreSQL

The provider determines whether the request has already been processed.

---

# Step 4 — Existing Request

If an idempotency record already exists:

1. The stored fingerprint is compared with the current request.
2. If the fingerprints match:
   - The stored response is returned immediately.
3. If the fingerprints differ:
   - An `IdempotencyFingerprintMismatchException` is thrown.

The endpoint is never executed a second time.

---

# Step 5 — New Request

If no record exists:

1. The request continues through the ASP.NET Core pipeline.
2. The endpoint executes normally.
3. The response is captured.
4. The response is persisted together with its fingerprint.
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

The middleware can terminate request processing under specific conditions.

| Condition | Result |
|----------|--------|
| Missing idempotency key | Request continues or fails depending on configuration |
| Fingerprint mismatch | `IdempotencyFingerprintMismatchException` |
| Storage provider failure | Storage-related exception |
| Response successfully replayed | Endpoint is skipped |

See **Errors** for detailed exception documentation.

---

# Performance Considerations

The middleware is optimized for high-throughput APIs.

For duplicate requests:

- Business logic is skipped.
- Database operations are avoided.
- The previously stored response is returned directly.

This significantly reduces processing time and prevents duplicate side effects.

---

# Related Documentation

- Architecture
- Fingerprinting
- Response Replay
- Storage Providers
- Observability
- Errors