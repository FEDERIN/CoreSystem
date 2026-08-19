# 🔄 CoreSystem.Cache.Rehydration

> **Cache recovery component for CoreSystem.Cache on .NET 8**

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Cache.Rehydration?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Cache.Rehydration?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

`CoreSystem.Cache.Rehydration` is an optional recovery component for
`CoreSystem.Cache`. It restores entries that were kept in the memory fallback
after the primary cache provider becomes healthy again.

The package does not implement a cache provider. It reads tracked entries from
the memory fallback and writes them to the current primary storage.

---

## 📦 CoreSystem Ecosystem

| Package | Responsibility |
|----------|----------------|
| **CoreSystem.Cache** | Cache orchestration, storage resolution and fallback support |
| **CoreSystem.Cache.Rehydration** | Recovery of tracked fallback entries into the primary storage |

`CoreSystem.Cache.Rehydration` depends on the abstractions and services provided
by `CoreSystem.Cache`.

An external primary cache storage implementing `IExternalCacheStorage` must be
registered before rehydration is enabled.

---

## 🚀 Getting Started

Register `CoreSystem.Cache` and an external primary cache provider first.

```csharp
services.AddCoreCache(options =>
{
    options.InstanceName = "my-app:";
});

services.AddCoreCacheRedis(options =>
{
    options.Configuration = redis =>
    {
        redis.EndPoints.Add("localhost", 6379);
    };
});
```

Then register rehydration:

```csharp
services.AddCoreCacheRehydration(options =>
{
    options.Enabled = true;
    options.Interval = TimeSpan.FromSeconds(30);
});
```

Rehydration registration requires:

- an enabled `Core.Cache` registration;
- an `IExternalCacheStorage` registration.

If the core cache is disabled, the rehydration services are not registered.
If the primary storage is not registered, registration throws an
`InvalidOperationException`.

---

## 🔄 How Recovery Works

The recovery process is driven by entries marked by the core cache with:

```csharp
CacheEntryOptions.Rehydrate
```

The memory fallback tracks those entries. During a rehydration cycle:

```text
Memory fallback
      │
      ▼
Tracked cache keys
      │
      ▼
MemoryRehydrationSource
      │
      ▼
CacheRehydrator
      │
      ▼
PrimaryRehydrationTarget
      │
      ▼
ICacheStorageResolver.Primary
```

An entry contains its key and value and can also contain its remaining
expiration and tags.

The primary storage receives the remaining expiration and tags when they are
available.

---

## ❤️ Primary Recovery Detection

Rehydration is not executed simply because the primary is currently healthy.

`RehydrationService` checks health checks tagged:

```text
primary
```

A recovery is detected only after the service has observed the primary as
unhealthy and subsequently observes it as healthy.

If no health check tagged `primary` exists, rehydration is not triggered.

After a successful recovery cycle, the component does not repeatedly
rehydrate while the primary remains healthy. A new rehydration requires another
unhealthy-to-healthy transition.

---

## ⚙️ Configuration

`RehydrationOptions` exposes the following settings:

| Option | Default | Description |
|--------|---------|-------------|
| `Enabled` | `true` | Enables registration of the rehydration services |
| `Interval` | `30 seconds` | Delay used by the background rehydration service |

Example:

```csharp
services.AddCoreCacheRehydration(options =>
{
    options.Enabled = true;
    options.Interval = TimeSpan.FromSeconds(30);
});
```

When `Enabled` is `false`, the options instance remains registered but the
rehydration source, target, rehydrator, service and hosted background service
are not registered.

---

## 🧠 Rehydration Behavior

The rehydration component processes tracked entries independently.

When an entry is successfully stored in the primary provider, it is removed
from the memory fallback.

If storing an entry fails:

- the failure is logged;
- the entry is not removed from the fallback;
- processing continues with the next entry;
- the entry remains available for a later rehydration cycle.

Expired or unavailable memory entries are ignored by the rehydration source.

---

## 🧩 Extensibility

The implementation separates the recovery source from the recovery target:

- `IRehydrationSource` obtains recoverable entries.
- `IRehydrationTarget` stores entries in the primary cache.
- `ICacheRehydrator` coordinates the transfer.

The current source and target abstractions are internal implementation
boundaries. The package does not currently expose a public provider SDK for
replacing the rehydration source or target.

---

## 🧪 Tests

The available unit tests cover:

- rehydration entry creation and optional properties;
- registration requirements;
- disabled-cache and disabled-rehydration behavior;
- primary storage requirements;
- memory entry tracking and extraction;
- expiration and tag preservation;
- expired-entry handling;
- storage into the primary provider;
- removal after successful storage;
- retention after storage failures;
- continuation after an individual entry failure;
- primary unhealthy-to-healthy recovery detection;
- background cycle execution and cancellation.

---

## 📚 Documentation

- [Getting Started](GettingStarted.md)
- [Basic Usage](BasicUsage.md)
- [Architecture](Architecture.md)
- [Configuration](Configuration.md)
- [Health Checks](HealthChecks.md)
- [Observability](Observability.md)
- [Extensibility](Extensibility.md)
- [Roadmap](Roadmap.md)

---

## 🗺️ Roadmap

The current implementation focuses on reliable recovery of tracked memory
fallback entries.

Potential future improvements include additional diagnostics, configurable
batch processing and additional recovery policies.

---

## 📄 License

MIT
