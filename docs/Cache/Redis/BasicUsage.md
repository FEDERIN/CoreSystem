# 🧑‍💻 Basic Usage

`CoreSystem.Cache.Redis` uses the same `ICoreCache` API exposed by
`CoreSystem.Cache`.

## Store Data

```csharp
await cache.SetAsync(
    "products:1",
    product,
    TimeSpan.FromMinutes(10));
```

The value is serialized and stored in Redis.

## Retrieve Data

```csharp
var product =
    await cache.GetAsync<Product>(
        "products:1");
```

A missing entry returns `null`.

## Cache-Aside

```csharp
var product =
    await cache.GetOrAddAsync(
        $"products:{id}",
        ct => repository.GetByIdAsync(id, ct),
        TimeSpan.FromMinutes(10));
```

For a missing key, the Redis provider uses a distributed lock before executing
the factory.

## Tags

```csharp
await cache.SetAsync(
    $"product:{id}",
    product,
    TimeSpan.FromMinutes(10),
    ["products"]);
```

Invalidate all entries associated with the tag:

```csharp
await cache.InvalidateByTagAsync("products");
```

## Remove an Entry

```csharp
await cache.RemoveAsync("products:1");
```

Removing an entry also removes its Redis tag-index information.

## Recovery

With the core fallback and `CoreSystem.Cache.Rehydration` enabled, entries
written to the memory fallback after a Redis failure can later be written back
to the Redis primary storage.
