# Why CoreSystem.Cache?

`IDistributedCache` is an excellent abstraction for storing and retrieving data from distributed cache providers. It provides a simple, provider-agnostic API that works well for many applications.

As distributed systems evolve, however, caching becomes more than storing and retrieving key/value pairs. Production applications can require resilience, observability, serialization, HTTP response caching, cache invalidation, distributed locking, and operational insights.

Implementing these capabilities independently around `IDistributedCache` can result in duplicated infrastructure code and inconsistent behavior across applications.

CoreSystem.Cache provides a unified caching platform that orchestrates these concerns while allowing applications to remain focused on business logic.

---

# The Problem

Modern distributed applications may require capabilities such as:

- Automatic failover when an external cache provider becomes unavailable.
- Cache recovery after connectivity is restored.
- Cache invalidation by logical groups.
- Distributed locking to prevent cache stampede.
- Consistent serialization across cache providers.
- HTTP response caching.
- OpenTelemetry metrics.
- Health monitoring.
- Extensibility without modifying application code.

These capabilities are not provided by a single basic cache abstraction. Teams can therefore end up building custom infrastructure around `IDistributedCache`.

---

# The Solution

CoreSystem.Cache acts as the orchestration layer of the CoreSystem caching ecosystem.

Cache operations flow through a configurable execution pipeline before reaching the selected storage provider. Cross-cutting concerns such as logging, metrics, resilience, and fallback can therefore be applied without placing that logic inside application services or storage implementations.

The framework can operate with its in-memory provider without requiring an external cache provider. When an external provider is configured, additional CoreSystem packages can extend the framework with capabilities such as Redis storage, resilience, and cache rehydration.

---

# Benefits

Using CoreSystem.Cache provides several advantages:

- Unified caching API across providers.
- Separation of business logic from infrastructure concerns.
- Cache-Aside support through `GetOrAddAsync`.
- Tag-based cache invalidation.
- Resilience and fallback support for external storage.
- Built-in observability and diagnostics.
- HTTP response caching support.
- Extensible execution pipeline.
- In-memory caching without requiring an external provider.

---

# When Should You Use CoreSystem.Cache?

CoreSystem.Cache is useful for applications that require more than basic key/value caching, including:

- High-performance APIs.
- Distributed systems.
- Microservices.
- Cloud-native applications.
- HTTP response caching.
- External cache providers such as Redis.
- Cache invalidation by logical groups.
- OpenTelemetry integration.
- Provider-independent caching.
- Resilience around external cache providers.

If your application only requires basic distributed key/value storage, the built-in `IDistributedCache` abstraction may be sufficient.

CoreSystem.Cache is designed for applications that need a unified caching API and additional caching capabilities that can be composed around the core cache pipeline.

---

# Next Steps

Continue with the **Architecture** section to understand how the execution pipeline is composed, how storage providers are selected, and how CoreSystem.Cache coordinates the different components of the caching framework.