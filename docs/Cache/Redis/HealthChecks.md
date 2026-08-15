# ❤️ Health Checks

`CoreSystem.Cache.Redis` registers a Redis health check through the
`IHealthCheckContributor` abstraction.

## Redis Check

The health check executes a Redis `PING` operation.

When Redis responds successfully:

```text
Healthy
Redis is connected successfully.
```

When Redis cannot be reached:

```text
Degraded
Redis is not responding. Memory fallback active.
```

The check is registered with the following name and tags:

```text
redis_cache
cache
primary
```

## Health State

`RedisHealthState` tracks Redis availability.

When Redis changes from healthy to unavailable, the provider records the
transition. When the connection becomes available again, the state changes back
to healthy.

The core fallback behavior can also mark the primary Redis storage unavailable
after a storage exception.

## Rehydration

Health recovery and cache rehydration are separate responsibilities.

`CoreSystem.Cache.Redis` reports the Redis health state. When the core fallback
stores an entry in memory with `CacheEntryOptions.Rehydrate`, the separate
`CoreSystem.Cache.Rehydration` package can later restore that entry to the
primary Redis provider.

Rehydration therefore depends on the external provider being registered and is
not itself a Redis health-check feature.
