# ⚡ CoreSystem.Memory

> **Lightweight in-memory synchronization library for .NET 8**

CoreSystem.Memory provides keyed asynchronous locks for coordinating
concurrent operations **within a single process**. It is lightweight,
thread-safe, dependency injection friendly, and designed for production
workloads.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Memory?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Memory?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

------------------------------------------------------------------------

## ✨ Features

-   ✅ Keyed asynchronous locking
-   ✅ Thread-safe implementation
-   ✅ Automatic lock lifecycle management
-   ✅ Lightweight and allocation-friendly
-   ✅ Dependency Injection integration
-   ✅ Designed for .NET 8
-   ✅ Production-ready

------------------------------------------------------------------------

## 📦 Installation

``` bash
dotnet add package CoreSystem.Memory
```

------------------------------------------------------------------------

## 🚀 Quick Start

Register the services:

``` csharp
builder.Services.AddCoreMemory();
```

Inject the lock provider:

``` csharp
public sealed class OrderService(IMemoryLockProvider lockProvider)
{
}
```

Acquire a keyed lock:

``` csharp
await using (await lockProvider.AcquireAsync($"orders:{orderId}"))
{
    // Critical section
}
```

------------------------------------------------------------------------

## 🏗 Typical Use Cases

-   Prevent concurrent processing of the same entity.
-   Protect critical sections inside ASP.NET Core applications.
-   Synchronize background jobs.
-   Avoid duplicate in-process operations.
-   Coordinate access to shared resources.

------------------------------------------------------------------------

## 📚 Design

Each key has its own asynchronous lock backed by `SemaphoreSlim`. Locks
are created on demand and automatically removed when no longer in use,
preventing unnecessary memory growth.

``` text
Application
      │
      ▼
IMemoryLockProvider
      │
      ▼
ConcurrentDictionary
      │
      ▼
SemaphoreSlim
```

------------------------------------------------------------------------

## ⚠️ Important

CoreSystem.Memory synchronizes execution **only inside the current
process**.

For synchronization across multiple application instances or servers,
use a distributed locking solution such as Redis.

------------------------------------------------------------------------

## 🤝 Contributing

Issues and pull requests are welcome.

------------------------------------------------------------------------

## 📄 License

Released under the MIT License.
