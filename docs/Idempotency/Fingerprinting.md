# 🔐 Fingerprinting

Fingerprinting is the mechanism that ensures an idempotency key can only be reused for **the same request**.

When a request is received, CoreSystem.Idempotency generates a fingerprint that uniquely identifies the request. If another request arrives with the same idempotency key but a different fingerprint, the request is rejected.

---

# Why Fingerprinting?

Without fingerprint validation, the same idempotency key could be accidentally reused for different operations.

For example:

**First request**

```http
POST /orders
X-Idempotency-Key: order-123

{
    "productId": 100,
    "quantity": 1
}
```

Later, the client accidentally sends:

```http
POST /orders
X-Idempotency-Key: order-123

{
    "productId": 100,
    "quantity": 5
}
```

Although both requests use the same idempotency key, they represent different business operations.

Request fingerprinting detects this difference and prevents the request from being processed.

---

# How It Works

For every incoming request, the middleware:

1. Reads the incoming HTTP request.
2. Generates a fingerprint.
3. Searches for the idempotency key.
4. Compares the stored fingerprint with the new fingerprint.

If both fingerprints are identical:

```text
Client
   │
   ▼
Generate Fingerprint
   │
   ▼
Stored Fingerprint?
   │
   ├── Same
   │      ▼
   │ Return cached response
   │
   └── Different
          ▼
Fingerprint Mismatch
```

---

# What Is Included?

By default, the fingerprint is generated using:

- HTTP method
- Request path
- Query string
- Request body

These values uniquely identify the logical operation.

---

# Included Headers

Some applications require specific headers to participate in fingerprint generation.

For example:

```csharp
builder.Services.AddCoreIdempotency(options =>
{
    options.Fingerprint.IncludedHeaders.Add("X-Tenant-Id");
    options.Fingerprint.IncludedHeaders.Add("X-Region");
});
```

Only the configured headers are included.

Headers that are not configured are ignored.

---

# When Should Headers Be Included?

Including headers is useful when they change the meaning of a request.

Examples include:

- Tenant identifiers
- Region identifiers
- API version headers

Avoid headers that change between retries, such as:

- Date
- User-Agent
- Trace identifiers
- Correlation identifiers

These values should not affect request identity.

---

# Fingerprint Mismatch

If the same idempotency key is reused with a different fingerprint, the middleware throws an:

```text
IdempotencyFingerprintMismatchException
```

This protects your application from replaying the response of a different request.

See **Errors → Fingerprint Mismatch** for troubleshooting and resolution steps.

---

# Configuration

Fingerprint generation can be customized through `FingerprintOptions`.

```csharp
builder.Services.AddCoreIdempotency(options =>
{
    options.Fingerprint.IncludedHeaders.Add("X-Tenant-Id");
});
```

For the complete list of configuration options, see **Configuration**.

---

# Best Practices

- Generate a new idempotency key for every logical operation.
- Keep the default fingerprint configuration whenever possible.
- Include only headers that define business identity.
- Never reuse an idempotency key for different requests.
- Do not include volatile headers that change between retries.