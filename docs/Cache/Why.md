# Why CoreSystem.Cache?

`IDistributedCache` is an excellent abstraction for storing and retrieving data from distributed cache providers. It provides a simple, provider-agnostic API that works well for many applications.

As distributed systems evolve, however, caching becomes much more than storing and retrieving key/value pairs. Production applications require resilience, observability, serialization, HTTP response caching, distributed locking, cache invalidation, and operational insights.

Implementing these capabilities independently in every application often results in duplicated infrastructure code, inconsistent behavior, and higher maintenance costs.

CoreSystem.Cache provides a unified caching platform that orchestrates these concerns while allowing applications to remain focused on business logic.

---

# The Problem

Modern distributed applications frequently require capabilities such as:

- Automatic failover when Redis becomes unavailable.
- Transparent cache recovery after connectivity is restored.
- Cache invalidation by logical groups.
- Distributed locking to prevent cache stampede.
- Consistent serialization across cache providers.
- HTTP response caching.
- OpenTelemetry metrics.
- Health monitoring.
- Extensibility without modifying application code.

Although these capabilities are common in production systems, they are rarely provided by a single caching abstraction. As a result, teams often build custom infrastructure around `IDistributedCache`, duplicating effort across projects.

---

# The Solution

CoreSystem.Cache acts as the orchestration layer of the CoreSystem caching ecosystem.

Instead of interacting directly with a storage provider, cache operations flow through a configurable execution pipeline that transparently applies cross-cutting concerns before reaching the selected provider.

This architecture coordinates specialized CoreSystem packages while exposing a single, unified API for application developers.

| Package | Responsibility |
|----------|----------------|
| **CoreSystem.Cache** | Cache orchestration, execution pipeline, HTTP middleware, Cache-Aside, resilience and observability |
| **CoreSystem.Cache.Memory** | In-memory cache provider |
| **CoreSystem.Cache.Redis** | Redis distributed cache provider |
| **CoreSystem.Serialization** | JSON, MessagePack and Protocol Buffers serialization |

Applications remain independent from provider-specific implementations while benefiting from a production-ready caching platform.

---

# Benefits

Using CoreSystem.Cache provides several advantages:

- Unified caching API across providers.
- Separation of business logic from infrastructure concerns.
- Automatic resilience and provider failover.
- Built-in observability and diagnostics.
- Consistent serialization across providers.
- HTTP response caching support.
- Reduced infrastructure duplication.
- Extensible execution pipeline.
- Production-ready defaults.

---

# When Should You Use CoreSystem.Cache?

CoreSystem.Cache is recommended for applications that require more than basic key/value storage, including:

- High-performance APIs.
- Distributed systems.
- Microservices.
- Cloud-native applications.
- HTTP response caching.
- Redis with automatic failover.
- Advanced cache invalidation.
- OpenTelemetry integration.
- Provider-independent caching.
- Production-grade resilience.

If your application only requires basic distributed key/value storage, the built-in `IDistributedCache` abstraction is often sufficient.

CoreSystem.Cache is designed for applications that need a complete, extensible, and production-ready caching platform.

---

# Next Steps

Continue with the **Architecture** section to understand how the execution pipeline is composed, how providers are selected, and how CoreSystem.Cache coordinates the different packages within the CoreSystem ecosystem.