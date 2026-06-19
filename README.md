# ⚙️ CoreSystem Ecosystem

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512bd4?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Architecture-Microservices-blue?style=for-the-badge" />
  <img src="https://img.shields.io/badge/OpenTelemetry-Native-orange?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Docker-Ready-2496ed?style=for-the-badge" />
  <img src="https://img.shields.io/badge/Status-Active-success?style=for-the-badge" />
</p>

<p align="center">
  <b>Cloud-Native • Modular • Observability-First • Production-Ready</b>
</p>

---

## 📖 Overview

**CoreSystem** is a modular ecosystem of reusable .NET libraries focused on simplifying the development of modern distributed systems and high-performance microservices.

The ecosystem provides production-ready building blocks for common cross-cutting concerns:

- Distributed observability
- Request idempotency
- Distributed caching
- Logging and tracing
- Infrastructure abstractions
- Cloud-native integrations
- Scalable API development

CoreSystem is built around:

- Clean Architecture principles
- Extensible designs
- Provider-based implementations
- Operational excellence
- Developer productivity

---

## 🌍 Ecosystem Vision

CoreSystem aims to become a complete toolkit for building scalable and reliable .NET distributed applications.

## Current Focus

- Observability
- Idempotency
- Distributed Caching

## Future Modules

- Messaging abstractions
- Security components
- Rate limiting
- Service discovery
- Resilience patterns
- API Gateway utilities

---

## ✨ Ecosystem Features

| Category | Description |
|---|---|
| 🏗 Modular Architecture | Independent reusable NuGet packages |
| 📊 Observability | OpenTelemetry-based telemetry ecosystem |
| ⚡ Performance | Optimized middleware-first components |
| 🧩 Extensibility | Provider-based architecture |
| ☁ Cloud-Native | Designed for distributed environments |
| 🚀 Developer Experience | Minimal configuration and integration |

---

## 🎯 Design Principles

CoreSystem follows a set of engineering principles:

- OpenTelemetry First
- Cloud-Native by Design
- Middleware-Centric Architecture
- Provider-Based Extensibility
- Production-Ready Defaults
- Low Coupling / High Cohesion
- Minimal Configuration
- Developer Experience Focused

---

## 🧠 Ecosystem Architecture

```mermaid
graph TD
    APIs --> CoreSystem

    CoreSystem --> Observability
    CoreSystem --> Idempotency
    CoreSystem --> DistributedCache

    Observability --> OpenTelemetry

    Idempotency --> PostgreSQL
    Idempotency --> Redis

    DistributedCache --> Memory
    DistributedCache --> Redis
```

---

## 🚀 Latest Releases

| Package | Latest Version |
|----------|----------|
| FGutierrez.Core.DistributedCache | 1.0.0 |
| FGutierrez.Core.Observability | 1.1.3 |
| FGutierrez.Core.Idempotency | 1.2.0 |

---

## 📦 Available Packages

| Package | Description | Status |
|---|---|---|
| `FGutierrez.Core.Observability` | OpenTelemetry, Serilog, Metrics & Health Checks | ✅ Stable |
| `FGutierrez.Core.Idempotency` | Distributed idempotency middleware | ✅ Stable |
| `FGutierrez.Core.DistributedCache` | High-performance distributed caching abstraction | ✅ Stable |

---

## ⚡ FGutierrez.Core.Observability

Production-grade observability integrations for ASP.NET Core applications.

### Features

- OpenTelemetry tracing
- Runtime metrics
- HTTP metrics
- Structured logging
- Serilog integration
- Health checks
- OTLP exporter support
- Automatic telemetry correlation

The library is backend-agnostic and can integrate with any OTLP-compatible observability platform.

---

## 🎟️ FGutierrez.Core.Idempotency

Distributed idempotency engine for ensuring critical operations execute exactly once.

### Useful for

- Payments
- Order creation
- Resource provisioning
- Distributed workflows

### Features

- Redis provider
- PostgreSQL provider
- Response replay
- Duplicate request prevention
- OpenTelemetry metrics
- Middleware-based execution pipeline
- Configurable expiration policies

---

## ⚡ FGutierrez.Core.DistributedCache

High-performance distributed caching library for .NET 8 providing a unified abstraction over multiple cache providers (Memory | Redis).

Provides a unified caching model over multiple providers:

- Memory Cache
- Redis

### Features

- Redis fallback strategy
- In-memory resilience mode
- Cache hit/miss metrics
- OpenTelemetry integration
- HTTP response caching middleware
- Cache-aside pattern support

---

## 🏗 Repository Structure

```text
CoreSystem/
│
├── .github/
│   └── workflows/
│
├── docs/
│
├── samples/
│   │
│   └── CoreSystem.Samples.Api/´
│       ├── Controllers/
│       ├── grafana/
│       ├── docker-compose.yml
│       ├── prometheus.yml
│       ├── otel-collector-config.yml
│       └── Program.cs
│
│   └── CoreSystem.Samples.Core/´
│       ├── Services/
│
├── src/
│   │
│   └── Core.DistributedCache/
│       ├── Abstractions/
│       ├── Diagnostics/
│       ├── Middleware/
│       ├── Options/
│       ├── Serialization/
│       ├── Storage/
│       │   ├── Memory/
│       │   └── Redis/
│       │   └── ResilientCacheDecorator.cs
│       ├── DistributedCacheExtensions.cs
│       ├── LICENSE
│       └── README.md
│   │
│   └── Core.Idempotency/
│       ├── Diagnostics/
│       ├── Middleware/
│       ├── Models/
│       ├── Options/
│       ├── Storage/
│       │   ├── PostgreSQL/
│       │   └── Redis/
│       └── CHANGELOG.md
│       ├── IdempotencyExtensions.cs
│       ├── IdempotencyRegistrationExtensions.cs
│       ├── LICENSE
│       └── README.md
│   │
│   ├── Core.Observability/
│   │   ├── Extensions/
│   │   ├── Options/
│   │   ├── ObservabilityDependencyInjection.cs
│   │   ├── LICENSE
│   │   └── README.md
│   │
│   ├── Core.Observability.Abstractions/
│       ├── IHealthCheckContributor.cs
│       ├── IObservabilityContributor.cs
│
├── tests/
│   │
│   └── Core.DistributedCache.Tests´
│       ├── Fixtures/
│       └── DistributedCacheExtensionsTests.cs
│
│   └── Core.Idempotency.Tests´
│       ├── Extensions/
│       ├── Fixtures/
│       ├── Middleware/
│       ├── Models/
│       ├── Options/
│       ├── Storage/
│
├── CHANGELOG.md
├── LICENSE
├── global.json
├── CoreSystem.sln
└── README.md
```

---

## 🏗 Technology Stack

### Backend

- .NET 8
- ASP.NET Core Minimal APIs
- Dapper
- Middleware Pipeline

### Observability

- OpenTelemetry
- Serilog
- OTLP Exporter

### Infrastructure

- PostgreSQL
- Redis
- Docker

---

## 🚀 Getting Started

### Clone Repository

```bash
git clone https://github.com/FEDERIN/CoreSystem.git
```

### Build Solution

```bash
dotnet build
```

### Run Sample API

```bash
cd samples/CoreSystem.Samples.Api
dotnet run
```

### Run Full Local Stack

```bash
docker compose up -d
```

This launches:

- PostgreSQL
- Redis
- OpenTelemetry Collector
- Sample dashboards and monitoring tools

---

## 📊 Telemetry Ecosystem

CoreSystem follows an **OpenTelemetry-first** approach.

Telemetry is exported through the OpenTelemetry Protocol (OTLP), making the ecosystem vendor-neutral and backend-agnostic.

Supported platforms include:

- Grafana
- Jaeger
- Prometheus
- Datadog
- New Relic
- Elastic
- Azure Monitor
- Any OTLP-compatible backend

```text
Application
      │
      ▼
OpenTelemetry SDK
      │
      ▼
OTLP Exporter
      │
      ▼
Collector / Backend
```

---

## 🧪 Engineering Principles

CoreSystem follows modern backend engineering practices:

- Clean Architecture
- SOLID Principles
- Middleware-First Integrations
- Provider-Based Extensibility
- High Cohesion / Low Coupling
- Production-Grade Defaults
- Cloud-Native Development
- Observability by Default

---

## 📌 Current Focus

### Completed

- [x] Distributed Observability
- [x] Idempotency Engine
- [x] Redis Support
- [x] PostgreSQL Support
- [x] OpenTelemetry Integration
- [x] Distributed Cache Foundation

### Planned

- [ ] Distributed Messaging
- [ ] JWT Security Components
- [ ] Rate Limiting
- [ ] API Gateway Utilities
- [ ] Kubernetes Helpers
- [ ] Service Discovery
- [ ] Resilience Policies

---

## 📦 Package Publishing

This repository uses GitHub Actions to automate NuGet packaging and publishing.

To publish a package, create a Git tag using the following format:

```text
<ProjectName>/v<Major>.<Minor>.<Patch>
```

Example:

```bash
git tag FGutierrez.Core.Idempotency/v1.2.0
git push origin FGutierrez.Core.Idempotency/v1.2.0
```

The GitHub workflow will automatically:

1. Build the package
2. Generate the NuGet artifact
3. Publish it to NuGet.org
4. Create the corresponding GitHub Release

---

## 🤝 Contributing

Contributions, ideas, and improvements are welcome.

## Development Workflow

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Open a Pull Request

---

## 📄 License

MIT License © Federin Pastor Gutierrez Ortiz

See the LICENSE file for details.

---

## ⭐ Support

If this ecosystem helps you, consider giving the repository a star on GitHub.

Building modern .NET distributed systems, one reusable component at a time.