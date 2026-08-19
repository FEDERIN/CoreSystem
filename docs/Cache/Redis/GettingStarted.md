# 🚀 Getting Started

This guide shows how to register Redis as the external cache provider for
`CoreSystem.Cache`.

## Prerequisites

Register `CoreSystem.Cache` before registering the Redis provider.

```csharp
services.AddCoreCache(options =>
{
    options.InstanceName = "my-app";
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

The Redis provider registers its implementation through the
`IExternalCacheStorage` contract.

## Resilience and Rehydration

Redis can be used together with the optional `CoreSystem.Resilience` and
`CoreSystem.Cache.Rehydration` packages.

When a Redis resilience pipeline is configured, the Redis provider integrates
with the corresponding resilience pipeline.

Rehydration is provided by the separate `CoreSystem.Cache.Rehydration` package
and is not configured through `AddCoreCacheRedis()`.
