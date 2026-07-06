# ⚡ CoreSystem.Cache

> **Production-ready distributed caching framework for .NET 8**

CoreSystem.Cache extends the standard .NET caching abstractions with a
production-ready execution pipeline, automatic Redis fallback, cache
rehydration, HTTP response caching, OpenTelemetry metrics, health
checks, and tag-based invalidation.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Cache?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Cache?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

------------------------------------------------------------------------

## ✨ Features

-   ✅ Memory and Redis providers
-   ✅ Cache-Aside (`GetOrAddAsync`)
-   ✅ Tag-based invalidation
-   ✅ Automatic Redis → Memory fallback
-   ✅ Automatic cache rehydration
-   ✅ HTTP response caching
-   ✅ OpenTelemetry metrics
-   ✅ ASP.NET Core Health Checks
-   ✅ Polly resilience integration
-   ✅ JSON, MessagePack and Protocol Buffers serialization

------------------------------------------------------------------------

## 📦 Installation

``` bash
dotnet add package CoreSystem.Cache
```

------------------------------------------------------------------------

## 🚀 Quick Start

Register the framework:

``` csharp
builder.Services.AddCoreCache(options =>
{
    options.Redis.Configuration = redis =>
    {
        redis.EndPoints.Add("localhost:6379");
    };
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
[Cacheable(expirationSeconds:300)]
public async Task<IActionResult> Get(Guid id)
{
    return Ok(await service.GetAsync(id));
}
```

------------------------------------------------------------------------

## 📊 Why CoreSystem.Cache?

  Capability               IDistributedCache   CoreSystem.Cache
  ----------------------- ------------------- ------------------
  Redis                           ✅                  ✅
  Memory Provider                 ❌                  ✅
  Cache-Aside                     ❌                  ✅
  Tag Invalidation                ❌                  ✅
  Automatic Fallback              ❌                  ✅
  Cache Rehydration               ❌                  ✅
  HTTP Response Caching           ❌                  ✅
  OpenTelemetry Metrics           ❌                  ✅
  Health Checks                   ❌                  ✅

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
      ├── Fallback
      └── Resilience
      │
      ▼
 Cache Storage
      ├── Redis
      └── Memory
```

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
