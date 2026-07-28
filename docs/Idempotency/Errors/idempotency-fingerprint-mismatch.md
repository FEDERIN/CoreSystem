# Idempotency Fingerprint Mismatch

**HTTP Status:** `409 Conflict`

**Exception**

```text
IdempotencyFingerprintMismatchException
```

**Problem Details Type**

```text
https://github.com/FEDERIN/CoreSystem/blob/main/docs/Idempotency/errors/idempotency-fingerprint-mismatch.md
```

---

## Summary

This error occurs when an incoming request reuses an existing **Idempotency-Key**, but the generated request fingerprint does not match the fingerprint originally associated with that key.

A request fingerprint is a deterministic hash generated from the incoming request based on the configured fingerprint options.

By default, the fingerprint is generated from the request body, but it can also include other request components such as headers, content type, query string, and additional values configured through `FingerprintOptions`.

For safety, CoreSystem.Idempotency only allows an idempotency key to be reused when the request is considered identical.

---

## Example

The following request is processed successfully and stored.

```http
POST /orders
Idempotency-Key: 15
Content-Type: application/json

{
    "amount": 100
}
```

Later, another request reuses the same idempotency key but changes the request payload.

```http
POST /orders
Idempotency-Key: 15
Content-Type: application/json

{
    "amount": 200
}
```

Since the generated fingerprint no longer matches the stored fingerprint, the middleware rejects the request and returns the following response.

```http
HTTP/1.1 409 Conflict
Content-Type: application/problem+json
```

```json
{
  "type": "https://github.com/FEDERIN/CoreSystem/blob/main/docs/Idempotency/errors/idempotency-fingerprint-mismatch.md",
  "title": "Idempotency fingerprint mismatch",
  "status": 409,
  "detail": "The request does not match the original request associated with this idempotency key.",
  "idempotencyKey": "15"
}
```

!!! note

    The response follows the ASP.NET Core **Problem Details** format (`application/problem+json`).

---

## Common Causes

This error is commonly caused by one of the following:

- The request body changed.
- The HTTP method changed.
- The request path changed.
- The `Content-Type` header changed.
- A configured request header changed.
- The query string changed (when included in the fingerprint).
- Different fingerprint options are being used between requests.

---

## Fingerprint Components

Depending on the configured `FingerprintOptions`, the request fingerprint may include one or more of the following request components:

- HTTP method
- Request path
- Query string
- Request body
- Content-Type
- Selected request headers

If any configured component changes while reusing the same idempotency key, the request fingerprint changes and this error is returned.

---

## Resolution

Choose one of the following approaches:

- Resend the **exact same request** using the existing idempotency key.
- Generate a **new idempotency key** if the request represents a different business operation.
- Verify the configured `FingerprintOptions` if identical requests unexpectedly produce different fingerprints.

---

## Why Is This Validation Required?

An idempotency key represents a single logical operation.

Allowing different requests to reuse the same idempotency key could result in:

- Returning an incorrect cached response.
- Executing an unintended business operation.
- Data inconsistencies.
- Duplicate financial or transactional operations.

To prevent these scenarios, CoreSystem.Idempotency validates both:

- The idempotency key.
- The generated request fingerprint.

Only when both values match can the previously stored response be replayed safely.

---

## Related Documentation

- Fingerprinting
- Response Replay
- Configuration
- Redis Provider
- PostgreSQL Provider