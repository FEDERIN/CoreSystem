# 🧩 Extensibility

One of the main goals of **CoreSystem.Cache** is to provide a caching framework that can evolve without requiring modifications to existing application code.

Rather than embedding infrastructure concerns directly into the cache service, the framework exposes extensibility points that allow new behaviors, storage providers, serializers, and other components to be added independently.

This document describes the available extension points and when to use each one.

---

# Extension Points

The framework can be extended in several ways.

| Extension Point | Purpose |
|-----------------|---------|
| Cache Pipeline | Add cross-cutting behaviors |
| Storage Providers | Support new cache backends |
| Serializers | Support additional serialization formats |
| Cache Key Generation | Customize cache key creation |
| Metrics | Publish custom telemetry |
| Dependency Injection | Replace default implementations |

---

# Extending the Cache Pipeline

Every cache operation flows through the pipeline.

```text
Application

↓

Cache Service

↓

Pipeline

↓

Storage
```

Because of this architecture, new behaviors can be introduced without modifying existing code.

Examples include:

- Compression
- Encryption
- Auditing
- Tracing
- Validation
- Custom Metrics

---

# Creating a Custom Behavior

Implement the `ICacheBehavior` interface.

```csharp
public sealed class CompressionBehavior
    : ICacheBehavior
{
    public async Task InvokeAsync(
        CacheContext context,
        CacheBehaviorDelegate next)
    {
        // Execute custom logic

        await next(context);

        // Execute additional logic
    }
}
```

Register the behavior.

```csharp
services.AddSingleton<ICacheBehavior, CompressionBehavior>();
```

The new behavior automatically becomes part of the execution pipeline.

---

# Creating a Custom Storage Provider

The framework supports multiple storage providers through the `ICacheStorage` abstraction.

```text
ICacheStorage

├── MemoryStorage

├── RedisStorage

└── YourCustomStorage
```

Implement the storage interface.

```csharp
public sealed class CosmosStorage
    : ICacheStorage
{
    // Implementation
}
```

Register it through dependency injection.

Once registered, the provider can participate in the storage resolution process.

---

# Creating a Custom Serializer

Serialization is provider independent.

Implement the serializer interface.

```csharp
public sealed class AvroSerializer
    : IPayloadSerializer
{
}
```

Then register the serializer.

```csharp
services.AddSingleton<IPayloadSerializer, AvroSerializer>();
```

Applications can switch serialization strategies without changing cache code.

---

# Replacing Default Services

Every core service is registered through dependency injection.

Applications can replace framework services with custom implementations when necessary.

Example:

```csharp
services.Replace(
    ServiceDescriptor.Singleton<
        ICacheKeyBuilder,
        CustomCacheKeyBuilder>());
```

---

# Custom Cache Key Generation

By default, cache keys are generated consistently across all providers.

Applications that require custom naming conventions can replace the key builder implementation.

Examples:

- Tenant-aware keys
- Region prefixes
- Environment prefixes
- Custom hashing

---

# Adding Custom Metrics

The framework exposes OpenTelemetry metrics.

Additional metrics can be collected through custom pipeline behaviors.

Examples:

- Business metrics
- SLA measurements
- Tenant metrics
- Custom counters

---

# Future Extension Points

The pipeline architecture enables additional capabilities without breaking the public API.

Examples include:

- CompressionBehavior
- EncryptionBehavior
- ValidationBehavior
- AuditBehavior
- TracingBehavior
- RateLimitingBehavior
- ReplicationBehavior
- WarmupBehavior

---

# Design Principles

When extending the framework, follow these principles:

- Single Responsibility Principle
- Prefer composition over inheritance
- Keep behaviors independent
- Avoid coupling to storage implementations
- Use dependency injection
- Keep custom behaviors stateless whenever possible

---

# Best Practices

✅ Create one behavior per concern.

✅ Do not modify existing framework behaviors.

✅ Prefer pipeline extensions over service replacement.

✅ Keep custom providers provider-independent.

✅ Preserve asynchronous execution.

---