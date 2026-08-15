# 🏗️ Architecture

`CoreSystem.Cache.Redis` implements the external storage contract consumed by
`CoreSystem.Cache`.

## Registration Flow

```text
AddCoreCache()
    │
    └── CacheOptions
          │
          ▼
AddCoreCacheRedis()
    │
    ├── Redis connection
    ├── RedisCacheStorage
    ├── RedisTagIndex
    ├── Redis health state
    └── Optional Redis resilience integration
          │
          ▼
Core.Cache
    │
    ├── Primary: Redis
    └── Fallback: Memory
          │
          ▼
Core.Cache.Rehydration
    └── Memory fallback → Primary Redis
```

`RedisCacheStorage` is registered as `IExternalCacheStorage`. The core
`CacheStorageResolver` therefore uses Redis as the primary storage and the
in-memory storage as fallback.

## Redis Storage

`RedisCacheStorage` implements:

- `GetAsync`
- `SetAsync`
- `RemoveAsync`
- `ExistsAsync`
- `InvalidateByTagAsync`
- `GetOrAddAsync`

Values are serialized through `IPayloadSerializer` before being stored in
Redis.

## Cache-Aside and Locking

`GetOrAddAsync()` first checks Redis. When the value is missing, it acquires a
distributed lock, checks Redis again, executes the factory, and stores the
generated value.

## Tags

`RedisTagIndex` maintains Redis sets for the relationship between cache keys and
tags. This supports tag invalidation and cleanup when entries are removed.

## Recovery

When the primary Redis storage fails, the core fallback behavior can execute
the operation against memory and mark the entry for rehydration. The separate
`CoreSystem.Cache.Rehydration` package later writes tracked fallback entries
back to the primary storage.
