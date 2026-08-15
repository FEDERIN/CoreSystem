# 🧑‍💻 Basic Usage

`CoreSystem.Cache.Rehydration` works with the fallback behavior implemented by
`CoreSystem.Cache`.

## Enable Rehydration

```csharp
services.AddCoreCacheRehydration(options =>
{
    options.Enabled = true;
    options.Interval = TimeSpan.FromSeconds(30);
});
```

An external primary cache provider must already be registered.

## Fallback Entries

The core cache marks fallback entries with:

```csharp
CacheEntryOptions.Rehydrate
```

The memory storage tracks these entries so that the rehydration source can find
them later.

The rehydration component preserves:

- the original key;
- the cached value;
- remaining expiration;
- cache tags.

## Recovery

The component does not immediately copy entries when the primary is unhealthy.

It waits for a recovery cycle in which the health checks tagged `primary`
report `Healthy` after a previous unhealthy state.

The tracked entries are then written to the primary provider and removed from
the memory fallback after successful storage.