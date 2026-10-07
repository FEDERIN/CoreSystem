# CoreSystem.Redis

> **Redis infrastructure and distributed locking for .NET 8.**

CoreSystem.Redis is a lightweight Redis infrastructure library built on
StackExchange.Redis. It provides reusable components for connection
management and distributed synchronization so cloud-native applications
and other CoreSystem modules can coordinate reliably across nodes.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Redis?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Redis?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

---

## Features

- Redis connection factory (`IRedisConnectionFactory` / `RedisConnectionFactory`)
- Distributed lock provider (`IDistributedLockProvider` / `RedisLockProvider`)
- Configurable lock options (`RedisLockOptions`)
- Dependency Injection integration

---

## Installation

```bash
dotnet add package CoreSystem.Redis
```

---

## Quick Start

```csharp
builder.Services.AddCoreRedis(options =>
{
    // Configure RedisLockOptions here.
});

public sealed class OrderService(IDistributedLockProvider lockProvider)
{
    public async Task ProcessAsync(CancellationToken ct)
    {
        await using var handle = await lockProvider.AcquireAsync("order-42", ct);
        // Only one instance executes this block at a time.
    }
}
```

> The connection string is resolved by `RedisConnectionFactory` from configuration.
> Consult the package README for the wire-up details.

---

## Relationship with CoreSystem.Memory

| | CoreSystem.Memory | CoreSystem.Redis |
| --- | --- | --- |
| Scope | Single process | Multiple nodes |
| Delivery | In-process semaphores | Redis distributed locks |
| Package | `CoreSystem.Memory` | `CoreSystem.Redis` |

---

## Contributing

Issues, discussions and pull requests are welcome.
