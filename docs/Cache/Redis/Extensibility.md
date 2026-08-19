# 🧩 Extensibility

`CoreSystem.Cache.Redis` is implemented as an external storage provider for
`CoreSystem.Cache`.

## Storage Integration

The provider implements the core `IExternalCacheStorage` contract through
`RedisCacheStorage`.

This allows the Redis provider to be registered as the external storage
implementation without changing the cache API.

## Key Building

Redis-specific key construction is isolated behind the internal `IKeyBuilder`
abstraction.

`RedisKeyBuilder` generates keys for:

- cache entries;
- tags;
- tag indexes;
- distributed locks.

The provider also applies the `CacheOptions.InstanceName` value as a key prefix.

## Tag Index

`RedisTagIndex` implements the core `ICacheTagIndex<RedisCacheStorage>` contract
and contains additional Redis-specific operations such as retrieving and
counting keys associated with a tag.

These types are internal implementation details; the current code does not
provide a public Redis provider SDK or public extension model for replacing
these components.

## Ecosystem Integration

Resilience and rehydration are extension points provided by separate packages:

- `CoreSystem.Resilience` supplies resilience pipelines.
- `CoreSystem.Cache.Rehydration` supplies fallback-entry recovery.

`CoreSystem.Cache.Redis` integrates with those components without exposing them
as Redis-specific extension APIs.
