# 🚀 Getting Started

This guide shows how to register Redis as the external cache provider for
`CoreSystem.Cache`.

## Prerequisites

Register `CoreSystem.Cache` before registering the Redis provider.

```csharp
services.AddCoreCache(options =>
{
    options.InstanceName = "my-app:";
});
```

Then register Redis:

```csharp
services.AddCoreCacheRedis(options =>
{
    options.Configuration = redis =>
    {
        redis.EndPoints.Add("localhost", 6379);
    };
});
```

`AddCoreCacheRedis()` requires the core `CacheOptions` registration and a Redis
configuration delegate. If either requirement is missing, registration throws
an `InvalidOperationException`.

## Using the Cache

Application services continue to use `ICoreCache`:

```csharp
public sealed class ProductService(ICoreCache cache)
{
    public Task<Product?> GetAsync(
        string key,
        CancellationToken ct = default)
        => cache.GetAsync<Product>(key, ct);
}
```

The Redis provider is selected internally by the core storage resolver.

## Resilience and Rehydration

Redis can be used together with the optional `CoreSystem.Resilience` and
`CoreSystem.Cache.Rehydration` packages.

The sample infrastructure configures resilience from `Core:Resilience` and
registers rehydration after the Redis provider. Rehydration requires an external
cache provider and uses the primary storage as its recovery target.
