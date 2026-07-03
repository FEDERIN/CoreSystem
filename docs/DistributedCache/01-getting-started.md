# 🚀 Getting Started

Welcome to **CoreSystem.Cache**, a production-ready distributed caching framework for **.NET 8**.

In this guide you'll learn how to:

- Install the package
- Register the framework
- Perform your first cache operations
- Use the built-in Cache-Aside pattern

> **Estimated time:** 5 minutes

---

# Prerequisites

Before getting started, ensure you have:

- .NET 8 SDK
- An ASP.NET Core application
- *(Optional)* A Redis server

---

# Step 1 — Install the Package

Install the NuGet package.

```bash
dotnet add package CoreSystem.Cache
```

---

# Step 2 — Register the Framework

Register the framework in your application's dependency injection container.

```csharp
builder.Services.AddCoreDistributedCache(options =>
{
    options.Redis.Configuration = redis =>
    {
        redis.EndPoints.Add("localhost:6379");
    };
});
```

> Looking for advanced configuration options?
>
> See **04-configuration.md**.

---

# Step 3 — Inject the Cache Service

Inject `ICoreCacheService` wherever caching is required.

```csharp
public sealed class ProductService(
    ICoreCacheService cache)
{
}
```

---

# Step 4 — Store a Value

```csharp
await cache.SetAsync(
    "products:1",
    product,
    TimeSpan.FromMinutes(10));
```

---

# Step 5 — Retrieve a Value

```csharp
var product = await cache.GetAsync<Product>(
    "products:1");
```

---

# Step 6 — Use the Cache-Aside Pattern

The recommended way to retrieve cached data is through `GetOrAddAsync`.

```csharp
var product = await cache.GetOrAddAsync(
    key: $"products:{id}",
    factory: async ct =>
        await repository.GetByIdAsync(id, ct),
    expiration: TimeSpan.FromMinutes(10),
    tags: ["products"]);
```

The framework automatically:

- Checks whether the item exists in the cache.
- Executes the data factory only on cache misses.
- Stores the result.
- Applies expiration.
- Executes the configured cache pipeline.

---

# Step 7 — Enable HTTP Response Caching (Optional)

If you want to cache HTTP responses, register the middleware.

```csharp
var app = builder.Build();

app.UseCoreDistributedCache();

app.Run();
```

Then decorate your endpoints.

```csharp
[HttpGet("{id}")]
[Cacheable(expirationSeconds:300)]
public async Task<IActionResult> Get(Guid id)
{
    return Ok(await service.GetAsync(id));
}
```

For a complete guide to HTTP response caching, see **06-http-cache.md**.

---

# Need Help?

If you encounter an issue or have a suggestion:

- Open a GitHub Issue.
- Start a Discussion.
- Submit a Pull Request.

Contributions are always welcome.
---