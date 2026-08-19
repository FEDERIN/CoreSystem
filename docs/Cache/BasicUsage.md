# 🧑‍💻 Basic Usage

This guide explains how to use the public API exposed by
`ICoreCache`.

By the end of this guide you'll know how to:

- Store values
- Retrieve values
- Remove entries
- Check if entries exist
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

An optional collection of tags can also be provided.

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

The factory executes when the cache entry does not exist.

The operation is handled by the configured cache provider and execution pipeline.

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

Tags can also be provided through `GetOrAddAsync()`.

---

# Invalidate a Tag

```csharp
await cache.InvalidateByTagAsync(
    "products");
```

All cache entries associated with that tag are invalidated.

---

# Working with CancellationToken

All asynchronous operations exposed by `ICoreCache` support cancellation.

```csharp
await cache.GetAsync<Product>(
    "products:1",
    cancellationToken);
```

The cancellation token is propagated through the cache operation.

---

# Typical Usage Pattern

```csharp
public async Task<Product?> GetAsync(
    Guid id,
    CancellationToken ct = default)
{
    return await cache.GetOrAddAsync(
        $"products:{id}",
        async cancellationToken =>
            await repository.GetByIdAsync(
                id,
                cancellationToken),
        expiration: TimeSpan.FromMinutes(15),
        tags: ["products"],
        ct: ct);
}
```

This is the recommended way to integrate the framework into application services.

---

# Best Practices

✅ Prefer `GetOrAddAsync()` over manually calling `GetAsync()` and `SetAsync()`.

✅ Use meaningful cache keys.

✅ Group related entries with tags.

✅ Configure sensible expiration values.

✅ Pass the `CancellationToken` from the calling operation.

✅ Avoid caching frequently changing data.