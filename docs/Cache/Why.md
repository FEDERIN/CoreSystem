# ❓ Why Another Distributed Cache?

`IDistributedCache` is an excellent abstraction for storing and retrieving data from distributed cache providers. It offers a simple, provider-agnostic API that works well for many applications.

However, as distributed systems grow in complexity, caching becomes much more than reading and writing key/value pairs.

Production applications often require resilience, observability, cache invalidation, distributed locking, HTTP response caching, serialization, and operational insights. These concerns are typically implemented separately in every project, leading to duplicated infrastructure code and inconsistent implementations.

**CoreSystem.Cache** is the orchestration layer of the **CoreSystem** caching ecosystem.

It provides a unified caching API while coordinating cache providers, the execution pipeline, HTTP response caching, resilience, observability, and serialization through dedicated CoreSystem packages.

---

# 📦 CoreSystem Ecosystem

CoreSystem.Cache integrates with specialized packages that provide storage providers and serialization while exposing a single, unified API for application developers.

| Package | Responsibility |
|----------|----------------|
| **CoreSystem.Cache** | Cache orchestration, execution pipeline, HTTP middleware, Cache-Aside, resilience and observability |
| **CoreSystem.Cache.Memory** | In-memory cache provider |
| **CoreSystem.Cache.Redis** | Redis distributed cache provider |
| **CoreSystem.Serialization** | JSON, MessagePack and Protocol Buffers serialization |

---

# The Problem

Modern distributed applications frequently require capabilities such as:

- Automatic failover when Redis becomes unavailable.
- Transparent recovery after connectivity is restored.
- Cache invalidation by logical groups.
- Distributed locking to prevent cache stampede.
- Consistent serialization across providers.
- HTTP response caching.
- OpenTelemetry metrics.
- Health monitoring.
- Extensibility without modifying application code.

Most teams end up implementing these features independently, increasing maintenance costs and introducing subtle inconsistencies between projects.

---

# The Solution

CoreSystem.Cache builds on top of the standard .NET distributed caching abstractions by introducing a composable execution pipeline that transparently manages infrastructure concerns.

Rather than coupling cache operations directly to a storage provider, every operation flows through a configurable execution pipeline where cross-cutting concerns are applied before reaching the selected cache provider.

This architecture allows business code to remain focused on application logic while the framework manages provider selection, resilience, serialization, observability, and HTTP response caching automatically.

---

# Feature Comparison

| Capability | `IDistributedCache` | CoreSystem Ecosystem |
|------------|:-------------------:|:--------------------:|
| Unified Cache API | ✅ | ✅ |
| Memory Cache Provider | ❌ | ✅ |
| Redis Cache Provider | ✅ | ✅ |
| Composable Execution Pipeline | ❌ | ✅ |
| Automatic Redis → Memory Fallback | ❌ | ✅ |
| Automatic Cache Rehydration | ❌ | ✅ |
| Cache-Aside Pattern | ❌ | ✅ |
| Tag-based Invalidation | ❌ | ✅ |
| Distributed Locking | ❌ | ✅ |
| Pluggable Serialization | ❌ | ✅ |
| HTTP Response Caching | ❌ | ✅ |
| OpenTelemetry Metrics | ❌ | ✅ |
| Health Checks | ❌ | ✅ |
| Provider Resolution | ❌ | ✅ |
| Extensible Architecture | Limited | ✅ |

---

# Design Philosophy

The framework follows a small set of architectural principles.

## Infrastructure Should Be Invisible

Applications should focus on business logic rather than cache implementation details.

CoreSystem.Cache automatically manages provider selection, serialization, resilience, observability, metrics, and fallback behavior.

---

## Extensibility First

New capabilities should be introduced without modifying existing code.

The composable execution pipeline allows behaviors such as compression, encryption, auditing, tracing, or custom metrics to be added independently of the core framework.

---

## Cloud-Native by Design

The framework embraces modern cloud-native development practices.

- OpenTelemetry integration
- ASP.NET Core Health Checks
- Dependency Injection
- Middleware-based integrations
- Provider abstraction
- Resilience patterns

---

## Modular by Design

Each responsibility lives in its own package.

This modular architecture allows applications to benefit from a unified caching experience while keeping providers, serialization, and infrastructure concerns independently maintainable.

---

## Production-Ready Defaults

The framework includes production-ready defaults while remaining fully configurable.

Examples include:

- Redis connectivity monitoring
- Automatic provider fallback
- Cache rehydration
- Distributed locking
- Configurable serialization
- Provider-independent API

---

# When Should You Use CoreSystem.Cache?

CoreSystem.Cache is an excellent fit when your application requires one or more of the following:

- High-performance APIs
- Microservices
- Cloud-native applications
- Distributed systems
- HTTP response caching
- OpenTelemetry observability
- Redis with automatic failover
- Provider-independent cache implementations
- Advanced caching strategies
- Production-grade resilience

---

# When Is `IDistributedCache` Enough?

If your application only requires basic distributed key/value storage without additional infrastructure capabilities, the built-in `IDistributedCache` abstraction is an excellent choice.

CoreSystem.Cache is intended for applications that require a complete, production-ready caching platform while maintaining a clean, modular, and extensible architecture.