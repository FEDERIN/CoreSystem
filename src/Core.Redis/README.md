# ⚡ CoreSystem.Redis

> **Redis infrastructure and distributed locking for .NET 8, 9 and 10.**

CoreSystem.Redis is a lightweight Redis infrastructure library built on
StackExchange.Redis. It provides reusable components for connection management
and distributed synchronization, so cloud-native applications and shared
libraries coordinate reliably across nodes.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Redis?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Redis?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-blue?style=for-the-badge)

---

## ✨ Features

- ✅ Redis connection factory with production-ready defaults
- ✅ Distributed lock provider
- ✅ Configurable `ConfigurationOptions`
- ✅ Dependency Injection integration
- ✅ Built on StackExchange.Redis
- ✅ Reusable across multiple libraries

---

## 📦 Installation

```bash
dotnet add package CoreSystem.Redis
```

---

## 🚀 Quick Start

Register the services:

```csharp
builder.Services.AddCoreRedis(options =>
{
    options.LockDuration = TimeSpan.FromSeconds(30);
    options.RetryDelay = TimeSpan.FromMilliseconds(50);
    options.MaxWaitTime = TimeSpan.FromSeconds(5);
});
```

Create a connection multiplexer:

```csharp
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var factory = sp.GetRequiredService<IRedisConnectionFactory>();

    return factory.Create(options =>
    {
        options.EndPoints.Add("localhost:6379");
    });
});
```

Acquire a distributed lock:

```csharp
public sealed class OrderService(IDistributedLockProvider lockProvider)
{
    public async Task ProcessAsync(Guid id, CancellationToken ct)
    {
        using var handle =
            await lockProvider.AcquireAsync($"orders:{id}", ct);

        // Critical section: only one holder across all nodes
    }
}
```

---

## 🧩 Included Components

| Component | Description |
|-----------|-------------|
| `IRedisConnectionFactory` | Creates Redis connections (implementation is internal). |
| `IDistributedLockProvider` | Abstraction for distributed locking (implementation is internal). |
| `RedisLockOptions` | Lock duration, retry delay and max wait time. |

---

## 🔒 Lock Behavior

`AcquireAsync` retries every `RetryDelay` until it takes the lock. If
`MaxWaitTime` is set and expires first, it throws `TimeoutException`. When
`MaxWaitTime` is null, it retries until the lock is taken or the token is
cancelled.

A lock is released when its handle is disposed. It also expires automatically
after `LockDuration`, so the critical section must finish within that window.

---

## 🏗 Architecture

```text
Application
      │
      ▼
IDistributedLockProvider
      │
      ▼
RedisLockProvider
      │
      ▼
RedisLock (LockTake / LockRelease)
      │
      ▼
Protected Operation
```

Each lock key is acquired atomically through Redis, so only one holder runs the
critical section at a time regardless of how many application instances exist.

---

## 📚 Documentation

- [Overview](https://federin.github.io/CoreSystem/Redis/)

---

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

---

## 📄 License

Released under the MIT License.
