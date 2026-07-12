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

  ----------------------------------------------------------------------------------
  Package                           Description                   Status
  --------------------------------- ----------------------------- ------------------
  CoreSystem.Cache                        Distributed cache with Memory ✅ Stable
                                    and Redis providers           

  CoreSystem.Idempotency                  HTTP request idempotency      ✅ Stable
                                    middleware                    

  CoreSystem.Memory                       In-process asynchronous keyed ✅ Stable
                                    locks                         

  CoreSystem.Redis                        Redis infrastructure and      ✅ Stable
                                    distributed locking           

  CoreSystem.Serialization                JSON, MessagePack and         ✅ Stable
                                    Protobuf abstraction          

  CoreSystem.Observability                Logging, Metrics, Tracing and ✅ Stable
                                    Health Checks                 

  CoreSystem.Observability.Abstractions   Extensibility contracts       ✅ Stable

  CoreSystem.Resilience                   Polly-based resilience        🚧 Coming Soon
                                    pipelines                     
  ----------------------------------------------------------------------------------

------------------------------------------------------------------------

# 🏗 Ecosystem Architecture

``` mermaid
graph TD

Application --> Cache
Application --> Idempotency
Application --> Observability

Cache --> Memory
Cache --> Redis
Cache --> Serialization

Idempotency --> Redis
Idempotency --> PostgreSQL

Observability --> OpenTelemetry

Resilience --> Polly
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
 ├── Core.Cache
 ├── Core.Idempotency
 ├── Core.Memory
 ├── Core.Redis
 ├── Core.Serialization
 ├── Core.Observability
 ├── Core.Observability.Abstractions
 └── Core.Resilience
```

------------------------------------------------------------------------

# 🛣 Roadmap

## Completed

-   Distributed Cache
-   Redis Infrastructure
-   Memory Synchronization
-   Serialization
-   Observability
-   Idempotency

## In Progress

-   Core.Resilience

## Planned

-   Messaging
-   Rate Limiting
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