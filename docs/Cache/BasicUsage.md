# 🧑‍💻 Basic Usage

This guide explains how to use the public API exposed by
`ICoreCache`.

By the end of this guide you'll know how to:

- Store values
- Retrieve values
- Remove entries
- Invalidate tags
- Use the Cache-Aside pattern
- Configure expiration

---

# Injecting the Cache Service

```csharp
public sealed class ProductService(
    ICoreCache cache)
{
}
```

---

# Store Data

Store an object in the cache.

```csharp
await cache.SetAsync(
    "products:1",
    product,
    TimeSpan.FromMinutes(10));
```

---

# Retrieve Data

```csharp
var product =
    await cache.GetAsync<Product>(
        "products:1");
```

Returns:

- the cached object
- or `null` if the entry does not exist.

---

# Check if an Entry Exists

```csharp
var exists =
    await cache.ExistsAsync(
        "products:1");
```

---

# Remove an Entry

```csharp
await cache.RemoveAsync(
    "products:1");
```

---

# Cache-Aside Pattern

The recommended approach for most scenarios.

```csharp
var product =
    await cache.GetOrAddAsync(
        key: $"products:{id}",
        factory: async ct =>
            await repository.GetByIdAsync(id, ct),
        expiration: TimeSpan.FromMinutes(10));
```

The factory executes only when the cache entry does not exist.

---

# Using Expiration

Expiration can be specified per operation.

```csharp
await cache.SetAsync(
    "products",
    products,
    TimeSpan.FromMinutes(5));
```

If omitted, the framework uses the configured default expiration.

---

# Using Tags

Tags allow related cache entries to be grouped together.

```csharp
await cache.SetAsync(
    key: $"product:{id}",
    value: product,
    expiration: TimeSpan.FromMinutes(10),
    tags: ["products"]);
```

---

# Invalidate a Tag

```csharp
await cache.InvalidateByTagAsync(
    "products");
```

All cache entries associated with that tag are removed.

---

# Working with CancellationToken

All asynchronous operations support cancellation.

```csharp
await cache.GetAsync<Product>(
    key,
    cancellationToken);
```

---

# Typical Usage Pattern

```csharp
public async Task<Product?> GetAsync(Guid id)
{
    return await cache.GetOrAddAsync(
        $"products:{id}",
        async ct =>
            await repository.GetByIdAsync(id, ct),
        expiration: TimeSpan.FromMinutes(15),
        tags: ["products"]);
}
```

This is the recommended way to integrate the framework into application services.

---

# Best Practices

✅ Prefer `GetOrAddAsync()` over manually calling `GetAsync()` and `SetAsync()`.

✅ Use meaningful cache keys.

✅ Group related entries with tags.

✅ Configure sensible expiration values.

✅ Avoid caching frequently changing data.