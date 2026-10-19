# 🧩 CoreSystem.Observability.Abstractions

> **Base contracts for health checks and observability contributors in .NET 8, 9 and 10**

CoreSystem.Observability.Abstractions defines the extension points that let
CoreSystem modules contribute health checks and OpenTelemetry activity sources
without depending on the observability runtime. It references only
`Microsoft.Extensions.Diagnostics.HealthChecks`; `CoreSystem.Observability`
discovers the registered contributors and wires them into the host.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Observability.Abstractions?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Observability.Abstractions?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-blue?style=for-the-badge)

------------------------------------------------------------------------

## ✨ Features

- ✅ `IHealthCheckContributor` contract for module health checks
- ✅ `IObservabilityContributor` contract for activity sources and DI registration
- ✅ Decoupled from the OpenTelemetry runtime
- ✅ Contributors are discovered by `CoreSystem.Observability` from dependency injection
- ✅ Targets .NET 8, .NET 9 and .NET 10

------------------------------------------------------------------------

## 📦 Installation

``` bash
dotnet add package CoreSystem.Observability.Abstractions
```

------------------------------------------------------------------------

## 🚀 Quick Start

Implement a health check contributor:

``` csharp
using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public sealed class DatabaseHealthCheckContributor : IHealthCheckContributor
{
    public void RegisterHealthChecks(
        IHealthChecksBuilder builder,
        IConfiguration configuration)
    {
        builder.AddCheck("database", () => HealthCheckResult.Healthy());
    }
}
```

Implement an observability contributor:

``` csharp
using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class OrdersObservabilityContributor : IObservabilityContributor
{
    public IEnumerable<string> GetActivitySources()
        => ["Orders.Api"];

    public void ConfigureObservability(
        IServiceCollection services,
        IConfiguration configuration)
    {
        // Register observability services used by the module.
    }
}
```

Register the contributors as **instances** so `CoreSystem.Observability` can
discover them:

``` csharp
builder.Services.AddSingleton<IHealthCheckContributor>(
    new DatabaseHealthCheckContributor());
builder.Services.AddSingleton<IObservabilityContributor>(
    new OrdersObservabilityContributor());
```

------------------------------------------------------------------------

## 🧩 Contributor Contracts

| Interface | Member | Purpose |
|---|---|---|
| `IHealthCheckContributor` | `RegisterHealthChecks(IHealthChecksBuilder, IConfiguration)` | Adds health checks to the health checks builder. |
| `IObservabilityContributor` | `GetActivitySources()` | Returns the activity source names contributed by the module. |
| `IObservabilityContributor` | `ConfigureObservability(IServiceCollection, IConfiguration)` | Registers observability services in the DI container. |

------------------------------------------------------------------------

## 🏗 How Discovery Works

Contributors are **not** discovered by scanning assemblies. Each module
registers its contributors as instances in the dependency injection container,
and `CoreSystem.Observability` reads them when the host is built:

``` text
Module
  │  services.AddSingleton<IHealthCheckContributor>(new MyContributor())
  ▼
CoreSystem.Observability
  │  reads the IServiceCollection descriptors
  ▼
Health checks and activity sources registered
```

> Note: discovery looks for registered **instances**. A type registration such
> as `AddSingleton<IHealthCheckContributor, MyContributor>()` is not picked up.

------------------------------------------------------------------------

## 📖 Documentation

The full documentation includes:

- Architecture overview
- Dependency injection guide
- Package dependency map

Visit the GitHub repository for the complete documentation.

------------------------------------------------------------------------

## 🤝 Contributing

Issues and pull requests are welcome.

------------------------------------------------------------------------

## 📄 License

Released under the MIT License.
