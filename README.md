# ⚙️ CoreSystem Ecosystem

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512bd4?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Architecture-Microservices-blue?style=for-the-badge" />
  <img src="https://img.shields.io/badge/OpenTelemetry-Native-orange?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Docker-Ready-2496ed?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Status-Active-success?style=for-the-badge" />
</p>

> **Production-ready modular infrastructure libraries for modern .NET
> applications.**

CoreSystem is an ecosystem of reusable .NET 8 libraries designed to
simplify the development of cloud-native, observable, and resilient
applications.

------------------------------------------------------------------------

# 🚀 Why CoreSystem?

CoreSystem provides production-ready building blocks for common
infrastructure concerns while keeping each package independent and
composable.

## Highlights

-   Modular architecture
-   OpenTelemetry-first
-   Dependency Injection friendly
-   Production-ready defaults
-   High performance
-   ASP.NET Core integration
-   Cloud-native
-   Minimal configuration

------------------------------------------------------------------------

# 📦 Ecosystem Packages

| Package | Description | Status |
| ------- | ----------- | ------ |
| [CoreSystem.Memory](https://www.nuget.org/packages/CoreSystem.Memory) | In-process asynchronous keyed locks | ✅ Stable |
| [CoreSystem.Redis](https://www.nuget.org/packages/CoreSystem.Redis) | Redis infrastructure and distributed locking | ✅ Stable |
| [CoreSystem.Serialization](https://www.nuget.org/packages/CoreSystem.Serialization) | JSON, MessagePack and Protobuf abstraction | ✅ Stable |
| [CoreSystem.Observability](https://www.nuget.org/packages/CoreSystem.Observability) | Logging, Metrics, Tracing and Health Checks | ✅ Stable |
| [CoreSystem.Observability.Abstractions](https://www.nuget.org/packages/CoreSystem.Observability.Abstractions) | Extensibility contracts | ✅ Stable |
| [CoreSystem.Resilience](https://www.nuget.org/packages/CoreSystem.Resilience) | Polly-based resilience pipelines | ✅ Stable |
| [CoreSystem.RateLimiting](https://www.nuget.org/packages/CoreSystem.RateLimiting) | Configurable rate limiting with metrics | ✅ Stable |
| [CoreSystem.Http](https://www.nuget.org/packages/CoreSystem.Http) | HTTP response capture and replay infrastructure | ✅ Stable |
| [CoreSystem.Http.ProblemDetails](https://www.nuget.org/packages/CoreSystem.Http.ProblemDetails) | RFC 9457 problem details and exception mapping | ✅ Stable |

External projects: [CoreSystem.Cache](https://www.nuget.org/packages/CoreSystem.Cache) and [CoreSystem.Idempotency](https://www.nuget.org/packages/CoreSystem.Idempotency) are maintained in their own repositories — see docs/External.

------------------------------------------------------------------------

# 🏗 Ecosystem Architecture

``` mermaid
graph TD

Application --> Http
Application --> RateLimiting
Application --> Resilience
Application --> Observability

Resilience --> Observability.Abstractions
RateLimiting --> Observability.Abstractions

Observability --> Observability.Abstractions
Observability --> OpenTelemetry
```

------------------------------------------------------------------------

# 🎯 Design Principles

-   OpenTelemetry First
-   Cloud Native by Design
-   Provider-Based Architecture
-   Middleware First
-   SOLID
-   Clean Architecture
-   Low Coupling
-   High Cohesion
-   Developer Experience

------------------------------------------------------------------------

# 🚀 Quick Start

``` bash
git clone https://github.com/FEDERIN/CoreSystem.git

dotnet build
```

------------------------------------------------------------------------

# 📁 Repository Structure

``` text
src/
 ├── Core.Http
 ├── Core.Http.ProblemDetails
 ├── Core.Memory
 ├── Core.Redis
 ├── Core.Serialization
 ├── Core.Observability
 ├── Core.Observability.Abstractions
 ├── Core.RateLimiting
 └── Core.Resilience
```

------------------------------------------------------------------------

# 🛣 Roadmap

## Completed

-   Memory Synchronization
-   Redis Infrastructure
-   Serialization
-   Observability
-   Resilience
-   Rate Limiting
-   HTTP infrastructure and Problem Details

## External / split out

-   [CoreSystem.Cache](https://www.nuget.org/packages/CoreSystem.Cache)
-   [CoreSystem.Cache.Redis](https://www.nuget.org/packages/CoreSystem.Cache.Redis)
-   [CoreSystem.Cache.Rehydration](https://www.nuget.org/packages/CoreSystem.Cache.Rehydration)
-   [CoreSystem.Idempotency](https://www.nuget.org/packages/CoreSystem.Idempotency)
-   [CoreSystem.Idempotency.Redis](https://www.nuget.org/packages/CoreSystem.Idempotency.Redis)
-   [CoreSystem.Idempotency.PostgreSql](https://www.nuget.org/packages/CoreSystem.Idempotency.PostgreSql)

## Planned

-   Messaging
-   Security
-   API Gateway utilities

------------------------------------------------------------------------

# 🤝 Contributing

Pull requests and suggestions are welcome.

------------------------------------------------------------------------
## 📄 License

MIT License © Federin Pastor Gutierrez Ortiz

See the LICENSE file for details.

---

## ⭐ Support

If this ecosystem helps you, consider giving the repository a star on GitHub.

Building modern .NET distributed systems, one reusable component at a time.
