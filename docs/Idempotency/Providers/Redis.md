# Redis Provider

The Redis provider stores idempotency records in a distributed cache, making it suitable for applications running on multiple instances.

It is the recommended provider for most production workloads that require low latency and horizontal scalability.

---

# When to Use Redis

Use the Redis provider when your application:

- Runs on multiple instances
- Is deployed to Kubernetes
- Requires low-latency request processing
- Needs distributed idempotency
- Can tolerate data loss after Redis persistence policies

If long-term durability is required, consider the PostgreSQL provider instead.

---

# Configuration

Configure Redis in your application settings.

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "Core": {
    "Idempotency": {
      "Provider": "Redis"
    }
  }
}
```

Register the framework:

```csharp
builder.Services.AddCoreIdempotency(options =>
{
    builder.Configuration
        .GetSection("Core:Idempotency")
        .Bind(options);
});
```

---

# How It Works

When a request reaches the middleware:

1. The request fingerprint is generated.
2. Redis is queried using the idempotency key.
3. If the key exists:
   - The stored fingerprint is validated.
   - The cached response is returned.
4. Otherwise:
   - The request executes normally.
   - The response is stored in Redis.
   - The response is returned to the client.

---

# Expiration

Stored responses automatically expire after the configured expiration period.

```csharp
options.Expiration =
    TimeSpan.FromHours(24);
```

After expiration, the request is treated as a new operation.

---

# Production Recommendations

✅ Deploy Redis in a highly available configuration.

✅ Configure persistence according to your recovery requirements.

✅ Monitor memory usage and eviction policies.

✅ Use dedicated Redis instances for production workloads whenever possible.

---

# Limitations

Redis is an in-memory data store.

Depending on its persistence configuration, data may be lost after an unexpected restart.

Applications that require durable storage should use the PostgreSQL provider.

---

# Related Documentation

- Configuration
- PostgreSQL Provider
- Fingerprinting
- Response Replay