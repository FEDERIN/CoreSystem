# Getting Started

Welcome to **CoreSystem**, a collection of production-ready .NET libraries designed to help you build modern, scalable, and maintainable applications.

## Prerequisites

Before getting started, ensure you have:

- .NET 8 SDK or later
- An IDE such as Visual Studio 2022 or JetBrains Rider
- Basic knowledge of C# and Dependency Injection

## Installation

Install the package you want to use.

Example:

```bash
dotnet add package CoreSystem.Cache
```

## Register the services

Register the package during application startup.

```csharp
builder.Services.AddCoreCache(options =>
{
    // Configure your options here
});
```

## First Example

```csharp
var value = await cache.GetOrAddAsync(
    "products",
    async () => await repository.GetProductsAsync(),
    TimeSpan.FromMinutes(5));
```

## Next Steps

Choose a package to continue learning.

- Cache
- Resilience
- Http
- Idempotency
- Memory
- Redis
- Serialization
- Observability