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
    └── IExternalCacheStorage
          │
          ▼
    RedisCacheStorage
```

`RedisCacheStorage` is registered as `IExternalCacheStorage`. The Redis provider
therefore supplies the external storage implementation consumed by the core
cache.

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

## Health and Resilience

The provider registers a Redis health check that verifies connectivity through
`PING` and maintains Redis health state.

When a Redis resilience pipeline is configured through `Core.Resilience`,
`Core.Cache.Redis` applies Redis connection and timeout exceptions to the
configured retry and circuit-breaker strategies.
