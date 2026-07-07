# CoreSystem.Redis

**CoreSystem.Redis** is a lightweight Redis infrastructure library for **.NET
8** built on top of **StackExchange.Redis**.

It provides reusable components for connection management and
distributed synchronization, making it easy to build reliable
cloud-native applications and reusable libraries.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Redis?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Redis?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![Redis](https://img.shields.io/badge/Redis-StackExchange.Redis-red?style=for-the-badge)

## Features

-   ✅ Redis Connection Factory
-   ✅ Distributed Lock Provider
-   ✅ Dependency Injection integration
-   ✅ Production-ready default configuration
-   ✅ Configurable `ConfigurationOptions`
-   ✅ Built on StackExchange.Redis
-   ✅ Designed for reuse across multiple libraries

## Installation

``` bash
dotnet add package Core.Redis
```

## Quick Start

Register the library:

``` csharp
builder.Services.AddCoreRedis();
```

Create a Redis connection:

``` csharp
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var factory = sp.GetRequiredService<IRedisConnectionFactory>();

    return factory.Create(options =>
    {
        options.EndPoints.Add("localhost:6379");
    });
});
```

Use distributed locks:

``` csharp
public sealed class OrderService(
    IDistributedLockProvider lockProvider)
{
    public async Task ProcessAsync(Guid id)
    {
        await using var handle =
            await lockProvider.AcquireAsync($"orders:{id}");

        // Critical section
    }
}
```

## Included Components

  Component                    Description
  ---------------------------- --------------------------------------
  `IRedisConnectionFactory`    Creates Redis connections.
  `RedisConnectionFactory`     Applies production-ready defaults.
  `IDistributedLockProvider`   Abstraction for distributed locking.
  `RedisLockProvider`          Redis-based lock implementation.
  `RedisLockOptions`           Lock configuration options.

## Designed For

-   ASP.NET Core applications
-   Distributed systems
-   Microservices
-   Background services
-   Shared infrastructure libraries

## Ecosystem

CoreSystem.Redis is the Redis infrastructure layer for the CoreSystem
ecosystem and is designed to be reused by libraries such as:

-   CoreSystem.Cache
-   CoreSystem.Idempotency
-   Future CoreSystem components

## License

MIT License.
