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
dotnet add package CoreSystem.Resilience
```

## Register the services

Register the package during application startup.

```csharp
builder.Services.AddCoreResilience(options =>
{
    // Configure your options here
});
```

## First Example

```csharp
var result = await pipeline.ExecuteAsync(async token =>
    await httpClient.GetFromJsonAsync<string>("https://example.com", token));
```

## Next Steps

Choose a package to continue learning.

- Resilience
- Rate Limiting
- Http
- Memory
- Redis
- Serialization
- Observability
- External projects (Cache, Idempotency)