# 🏗️ Architecture

`CoreSystem.Cache.Rehydration` coordinates recovery between the memory fallback
and the primary cache provider.

## Rehydration Flow

```text
Core.Cache
    │
    ├── Primary provider
    │
    └── Memory fallback
          │
          │ CacheEntryOptions.Rehydrate
          ▼
    ICacheKeyTracker
          │
          ▼
MemoryRehydrationSource
          │
          ▼
RehydrationService
          │
          │ Primary health = Healthy
          ▼
CacheRehydrator
          │
          ▼
PrimaryRehydrationTarget
          │
          ▼
ICacheStorageResolver.Primary
```

## Source

`MemoryRehydrationSource` reads tracked keys from memory and creates
`CacheRehydrationEntry` objects containing:

- key;
- value;
- remaining expiration;
- tags.

Expired entries are ignored.

## Target

`PrimaryRehydrationTarget` writes each entry to the current primary storage,
preserving its remaining expiration and tags.

## Recovery Detection

`RehydrationService` executes a health-check cycle and considers the primary
healthy only when at least one health check tagged `primary` exists and all such
checks report `Healthy`.

Rehydration starts only after the service has previously observed the primary as
unhealthy and then observes it as healthy.

## Background Processing

`RehydrationBackgroundService` executes the recovery cycle repeatedly using the
configured interval.

`CacheRehydrator` processes entries in batches of 100 and waits 100 ms between
batches. A failed entry is logged and remains available for a later attempt.
