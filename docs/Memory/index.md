# CoreSystem.Memory

> **Keyed asynchronous locks for in-process coordination.**

CoreSystem.Memory provides lightweight, production-ready keyed asynchronous locks for coordinating
concurrent work **within a single process** — ideal for in-memory caches, deduplication guards,
and per-resource synchronisation that does not justify a distributed lock.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Memory?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Memory?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

---

## Features

- Keyed asynchronous locking
- Thread-safe implementation
- Automatic lock lifecycle management
- Lightweight and allocation-friendly
- Dependency Injection integration
- Designed for .NET 8

---

## Installation

```bash
dotnet add package CoreSystem.Memory
```

---

## Quick Start

```csharp
builder.Services.AddCoreMemory();

// ...

public sealed class OrderService(IAsyncKeyLock lockProvider)
{
    public async Task SerializeOrderAsync(string orderId, CancellationToken ct)
    {
        await using var handle = await lockProvider.AcquireAsync(orderId, ct);
        await DoCriticalWorkAsync(ct);
    }
}
```

---

## Typical Use Cases

- Serialising access to a shared in-memory cache entry per key
- Preventing duplicate background work for the same logical resource
- Coordinating read/write access inside a single-node service

---

## Design

The lock provider builds one `SemaphoreSlim` per key on demand. Keys' resources
are reference-counted and garbage-collected once unused, keeping memory bounded
in long-running services. Use a distributed lock (e.g., `CoreSystem.Redis`) when
synchronisation must span multiple nodes.
