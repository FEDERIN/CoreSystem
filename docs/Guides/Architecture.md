# CoreSystem Architecture

## Overview

CoreSystem is a modular ecosystem of .NET libraries designed to simplify the development of modern applications.

Each package has a single responsibility and integrates seamlessly with the rest of the ecosystem through dependency injection, abstractions, and shared design principles.

The primary goals of CoreSystem are:

* Modularity
* Performance
* Extensibility
* Observability
* Maintainability
* Consistent developer experience

---

## Design Principles

CoreSystem follows a consistent set of architectural principles across all packages.

### Single Responsibility

Each package focuses on solving a specific problem.

Examples:

* **CoreSystem.RateLimiting** provides rate limiting.
* **CoreSystem.Resilience** provides resilience pipelines.
* **CoreSystem.Http** provides reusable HTTP infrastructure.
* **CoreSystem.Serialization** provides serialization services.
* **CoreSystem.Observability** provides application observability.

---

### Modular Architecture

Packages are designed to be consumed independently whenever possible.

Applications only install the packages they require.

---

### Dependency Injection First

Every package integrates naturally with the Microsoft dependency injection ecosystem.

```csharp
builder.Services.AddCoreRateLimiting();

builder.Services.AddCoreResilience();

builder.Services.AddCoreObservability();
```

---

### Extensibility

CoreSystem exposes public abstractions that allow applications to customize behavior without modifying the framework.

Extension points are implemented through contracts and dependency injection.

---

### Optional Components

CoreSystem is intentionally modular.

Not every application requires every package.

For example:

* An application may use **CoreSystem.Serialization** without **CoreSystem.Observability**.
* An application may use **CoreSystem.Http** independently.
* An application may only require **CoreSystem.Resilience**.

Applications decide which packages to include based on their requirements.

---

## Ecosystem Overview

```text
                           Applications
                                  │
        ┌─────────────────────────┼─────────────────────────┐
        │                         │                         │
        ▼                         ▼                         ▼
   CoreSystem.Http     CoreSystem.Memory      CoreSystem.Serialization
   CoreSystem.RateLimiting      CoreSystem.Redis    CoreSystem.Http.ProblemDetails
        │                         │                         │
        ▼                         ▼                         ▼
   CoreSystem.Resilience ──────►  CoreSystem.Observability.Abstractions
                                  ▲
                                  │
                        CoreSystem.Observability
```

The diagram illustrates the logical organization of the CoreSystem ecosystem.

Packages collaborate through abstractions while remaining loosely coupled.

Applications decide which optional packages should be registered.

---

## Shared Contracts

Some capabilities are shared through abstraction packages.

### CoreSystem.Observability.Abstractions

This package defines the contracts used by the observability infrastructure.

Examples include:

* Observability contributors
* Health check contributors

The package contains interfaces only and does not provide runtime implementations.

---

## Optional Infrastructure

### CoreSystem.Observability

CoreSystem.Observability provides a complete implementation for application observability.

Features include:

* OpenTelemetry integration
* Metrics
* Distributed tracing
* Health check registration
* Automatic discovery of observability contributors

CoreSystem.Observability is part of the CoreSystem ecosystem, but its use is optional.

Applications decide whether to register the package based on their monitoring requirements.

This approach keeps the remaining packages lightweight while still providing a rich observability experience when needed.

---

## Package Responsibilities

### CoreSystem.RateLimiting

Responsibilities:

* Rate limiting policies
* Per-user / per-IP throttling
* Rate limiting metrics
* Extensibility

---

### CoreSystem.Resilience

Responsibilities:

* Retry
* Timeout
* Circuit breaker
* Resilience pipelines

---

### CoreSystem.Http

Responsibilities:

* Shared HTTP infrastructure
* Response capture
* Response writing
* HTTP utilities

---

### CoreSystem.Http.ProblemDetails

Responsibilities:

* RFC 9457 problem details
* Exception-to-problem mapping
* Correlation identifiers
* Error response middleware

---

### CoreSystem.Memory

Responsibilities:

* In-memory synchronization
* Keyed asynchronous locks

---

### CoreSystem.Redis

Responsibilities:

* Redis integration
* Distributed locking support

---

### CoreSystem.Serialization

Responsibilities:

* Serialization abstractions
* JSON serialization
* Serializer implementations

---

### CoreSystem.Observability

Responsibilities:

* Metrics
* Tracing
* OpenTelemetry integration
* Health check registration
* Observability bootstrap

---

## Architecture Layers

CoreSystem can be viewed as a layered architecture.

```text
Applications
      │
      ▼
Feature Packages
(RateLimiting, Http.ProblemDetails)

      │
      ▼
Infrastructure Packages
(Http, Resilience)

      │
      ▼
Foundation Packages
(Memory, Redis, Serialization)

      │
      ▼
Shared Contracts
(CoreSystem.Observability.Abstractions)

      │
      ▼
Optional Infrastructure
(CoreSystem.Observability)
```

These layers describe the conceptual organization of the ecosystem rather than strict project references.

---

## Architecture Goals

CoreSystem is designed to provide:

* Modular architecture
* Clean separation of concerns
* Minimal dependencies
* High performance
* Low memory allocations
* Native AOT compatibility
* OpenTelemetry support
* Easy extensibility
* Production-ready infrastructure
* Consistent developer experience

---

## Next Steps

After understanding the overall architecture, continue with the documentation for each package.

* CoreSystem.RateLimiting
* CoreSystem.Resilience
* CoreSystem.Http
* CoreSystem.Http.ProblemDetails
* CoreSystem.Memory
* CoreSystem.Redis
* CoreSystem.Serialization
* CoreSystem.Observability
