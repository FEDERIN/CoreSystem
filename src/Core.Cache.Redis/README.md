# ⚡ CoreSystem.Cache

> **Production-ready distributed caching framework for .NET 8**

CoreSystem.Cache provides a unified cache abstraction with an execution
pipeline, Memory caching, optional external cache storage with fallback,
HTTP response caching, OpenTelemetry metrics, and tag-based invalidation.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Cache?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Cache?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

------------------------------------------------------------------------

## ✨ Features

-   ✅ Memory cache provider
-   ✅ Cache-Aside (`GetOrAddAsync`)
-   ✅ Tag-based invalidation
-   ✅ Optional external cache with Memory fallback
-   ✅ HTTP response caching
-   ✅ OpenTelemetry metrics for cache hits and misses
-   ✅ Configurable execution pipeline
-   ✅ Optional resilience integration through `Core.Resilience`
-   ✅ Configurable serialization through `Core.Serialization`

Redis support is provided by the `CoreSystem.Cache.Redis` package, which
registers Redis as the external primary storage and uses Memory as the
fallback storage.

------------------------------------------------------------------------

## 📦 Installation

``` bash
dotnet add package CoreSystem.Cache
```

For Redis support, add the Redis provider package separately:

``` bash
dotnet add package CoreSystem.Cache.Redis
```

------------------------------------------------------------------------

## 🚀 Quick Start

Register the framework:

``` csharp
builder.Services.AddCoreCache(options =>
{
    options.DefaultExpiration = TimeSpan.FromMinutes(30);
});
```

Inject the cache service:

``` csharp
public sealed class ProductService(ICoreCache cache)
{
}
```

Store data:

``` csharp
await cache.SetAsync(
    "products:1",
    product,
    TimeSpan.FromMinutes(10));
```

Retrieve data:

``` csharp
var product = await cache.GetAsync<Product>("products:1");
```

Recommended Cache-Aside pattern:

``` csharp
var product = await cache.GetOrAddAsync(
    $"products:{id}",
    async ct => await repository.GetByIdAsync(id, ct),
    expiration: TimeSpan.FromMinutes(10),
    tags: ["products"]);
```

------------------------------------------------------------------------

## 🌐 HTTP Response Caching

Enable the middleware:

``` csharp
app.UseCoreCache();
```

Decorate your endpoint:

``` csharp
[Cacheable(expirationSeconds: 300)]
public async Task<IActionResult> Get(Guid id)
{
    return Ok(await service.GetAsync(id));
}
```

HTTP response caching is applied only to endpoints decorated with
`CacheableAttribute`. The default request policy allows `GET` and `HEAD`
requests and excludes requests containing an `Authorization` header.

------------------------------------------------------------------------

## 📊 Why CoreSystem.Cache?

| Capability | `IDistributedCache` | CoreSystem.Cache |
|---|---:|---:|
| Memory Provider | ❌ | ✅ |
| Cache-Aside | ❌ | ✅ |
| Tag Invalidation | ❌ | ✅ |
| Primary + Fallback Storage | ❌ | ✅ |
| HTTP Response Caching | ❌ | ✅ |
| OpenTelemetry Metrics | ❌ | ✅ |
| Configurable Cache Pipeline | ❌ | ✅ |

Redis and resilience capabilities are provided through their corresponding
CoreSystem packages and are integrated with the cache pipeline when
registered and configured.

------------------------------------------------------------------------

## 🏗 Architecture

``` text
Application
      │
      ▼
 ICoreCache
      │
      ▼
 CachePipeline
      │
      ├── Logging
      ├── Metrics
      ├── Fallback (when available)
      └── Resilience (when registered)
      │
      ▼
 Cache Storage Resolver
      │
      ├── External Storage (Primary)
      │
      └── Memory Storage (Fallback)
```

When no external cache storage is registered, Memory is used as the primary
storage. When one external storage is registered, it becomes the primary
storage and Memory becomes the fallback. The core resolver allows only one
external cache storage to be registered.

------------------------------------------------------------------------

## 📚 Documentation

The full documentation includes:

-   Getting Started
-   Architecture
-   Configuration
-   Basic Usage
-   HTTP Response Caching
-   Observability
-   Health Checks
-   Extensibility
-   Roadmap

Visit the GitHub repository for the complete documentation.

------------------------------------------------------------------------

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

------------------------------------------------------------------------

## 📄 License

Released under the MIT License.
