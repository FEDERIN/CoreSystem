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

The provider also exposes `IPrimaryHealthStateWriter`, allowing the core cache
integration to mark Redis as unavailable when that integration detects a
primary-storage failure.

## Rehydration

Health recovery and cache rehydration are separate responsibilities.

`CoreSystem.Cache.Redis` reports the Redis health state. The separate
`CoreSystem.Cache.Rehydration` package can restore fallback entries to the
primary storage when configured.

Rehydration therefore depends on the external provider being registered and is
not itself a Redis health-check feature.
