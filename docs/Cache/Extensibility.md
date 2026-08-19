# 🧩 Extensibility

One of the main goals of **CoreSystem.Cache** is to keep cache operations separated from storage and cross-cutting concerns.

The current implementation provides extensibility through its pipeline and dependency injection structure, while some extension points described in earlier documentation are internal to the framework and are therefore not public extension APIs.

---

# Extension Points

The current implementation provides these relevant extension points:

| Extension Point | Current Support |
|-----------------|-----------------|
| Cache Pipeline | Pipeline behaviors are defined through `ICacheBehavior`. |
| Storage Providers | Storage is abstracted through `ICacheStorage`, but the interface is internal. |
| Serialization | Serialization is delegated to `CoreSystem.Serialization`. |
| HTTP Policies | Request and response cache policies are represented by internal abstractions. |
| Cache Key Generation | HTTP key generation is represented by an internal abstraction. |
| Dependency Injection | Framework services are registered through the built-in registration methods. |

---

# Extending the Cache Pipeline

Every cache operation is executed through `CachePipeline`.

```text
Application
    ↓
ICoreCache
    ↓
CacheContext
    ↓
CachePipeline
    ↓
ICacheStorage
```

Pipeline behaviors implement:

```csharp
public interface ICacheBehavior
{
    int Order { get; }

    Task InvokeAsync(
        CacheContext context,
        CacheDelegate next);
}
```

The current framework includes behaviors for logging, metrics, resilience, and fallback.

---

# Custom Pipeline Behaviors

`ICacheBehavior` is public and can be implemented by another component.

```csharp
public sealed class CustomBehavior
    : ICacheBehavior
{
    public int Order => 500;

    public async Task InvokeAsync(
        CacheContext context,
        CacheDelegate next)
    {
        await next(context);
    }
}
```

However, the current `AddCachePipeline()` implementation explicitly builds the pipeline from the framework's registered behaviors.

Therefore, registering:

```csharp
services.AddSingleton<ICacheBehavior, CustomBehavior>();
```

does **not** by itself add the custom behavior to the current pipeline.

Custom behaviors require an extension or replacement of the pipeline registration.

---

# Creating a Custom Storage Provider

The cache operations use the internal `ICacheStorage` abstraction.

```csharp
internal interface ICacheStorage
{
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken ct = default);

    Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions? options = null,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default);

    Task RemoveAsync(
        string key,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(
        string key,
        CancellationToken ct = default);

    Task InvalidateByTagAsync(
        string tag,
        CancellationToken ct = default);

    Task<T?> GetOrAddAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CacheEntryOptions? options = null,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default);
}
```

The abstraction is intentionally internal to `CoreSystem.Cache`.

This means an external application cannot currently implement a custom provider directly against this interface as a public SDK.

External providers are integrated through the CoreSystem cache/provider architecture.

---

# Storage Resolution

`ICacheStorageResolver` determines the primary and fallback storage.

```csharp
internal interface ICacheStorageResolver
{
    ICacheStorage Primary { get; }

    ICacheStorage? Fallback { get; }

    bool HasFallback { get; }
}
```

The current resolver uses:

```text
No external provider
    Primary  → MemoryStorage
    Fallback → None

External provider
    Primary  → External provider
    Fallback → MemoryStorage
```

This keeps provider selection outside `ICoreCache`.

---

# Serialization

Serialization is delegated to the CoreSystem serialization infrastructure.

`CacheOptions` selects the serializer through:

```csharp
options.SerializerType =
    SerializerType.Json;
```

The current `CoreSystem.Cache` implementation does not define a public serializer extension interface of its own.

Custom serialization strategies therefore belong to the serialization component rather than to the cache orchestration layer.

---

# HTTP Extension Points

HTTP caching uses separate abstractions for request and response policies:

```csharp
IRequestCachePolicy
IResponseCachePolicy
ICacheKeyGenerator
IHttpCacheHandler
```

These interfaces are internal to the current `CoreSystem.Cache` implementation.

The default implementations define the current HTTP caching behavior without exposing them as public customization contracts.

---

# Dependency Injection

`CoreSystem.Cache` registers its internal services through `AddCoreCache()`.

```csharp
builder.Services.AddCoreCache(options =>
{
});
```

The registration creates the cache pipeline, Memory storage, HTTP cache services, diagnostics, storage resolver, and `ICoreCache`.

Because several of the underlying interfaces are internal, dependency injection should not currently be considered a complete public extension SDK.

---

# Adding Custom Metrics

The framework exposes `CacheMetrics` and integrates it with OpenTelemetry.

The current built-in metrics are:

```text
cache.distributed.hits
cache.distributed.misses
```

Additional application metrics can be implemented independently using the application's own telemetry infrastructure.

The current cache pipeline does not automatically discover arbitrary `ICacheBehavior` registrations, so custom pipeline telemetry requires extending or replacing the pipeline registration.

---

# Future Extension Points

The architecture can support additional capabilities in future versions.

Possible extensions include:

- Public provider SDK
- Configurable pipeline behaviors
- Custom HTTP cache policies
- Public cache key generation contracts
- Additional cache providers
- Compression behaviors
- Encryption behaviors
- Tracing behaviors
- Validation behaviors

These should be treated as future capabilities rather than current public extension APIs.

---

# Design Principles

When extending the framework:

- Keep storage concerns separate from cache orchestration.
- Prefer composition over changes to `ICoreCache`.
- Keep pipeline behaviors focused on a single concern.
- Preserve asynchronous execution and cancellation.
- Avoid coupling application code to provider implementations.
- Keep the public cache API small.

---

# Technical Assessment

The current architecture has a good internal foundation for extensibility, particularly around `ICacheBehavior`, `ICacheStorage`, and `ICacheStorageResolver`.

The main limitation is that several of these abstractions are internal, and the current pipeline registration explicitly selects the built-in behaviors.

Therefore, **CoreSystem.Cache is internally extensible today, but it does not yet expose a complete public extension SDK for external developers**.

That distinction is important for the documentation because it accurately reflects the current implementation without promising extension mechanisms that the code does not currently provide.
