# 🗺️ Roadmap

This document outlines the planned evolution of **CoreSystem.Cache**.

The roadmap is intended to provide visibility into the long-term direction of the project. Features may evolve based on community feedback and real-world adoption.

---

# Guiding Principles

The framework will continue to evolve following these principles:

- Production-first
- Cloud-native
- Provider-independent
- Extensible architecture
- OpenTelemetry-first
- Backward compatibility whenever possible

---

# ✅ Completed

The following capabilities are already available.

## Core Infrastructure

- [x] Memory cache provider
- [x] Redis cache provider
- [x] Provider abstraction
- [x] Dependency Injection integration

---

## Cache Pipeline

- [x] Composable execution pipeline
- [x] Logging behavior
- [x] Metrics behavior
- [x] Resilience behavior
- [x] Fallback behavior

---

## Caching Features

- [x] Cache Aside pattern (`GetOrAddAsync`)
- [x] Tag-based invalidation
- [x] Distributed locking
- [x] Automatic cache rehydration
- [x] HTTP response caching

---

## Serialization

- [x] JSON
- [x] MessagePack
- [x] Protocol Buffers

---

## Observability

- [x] OpenTelemetry Metrics
- [x] Health Checks

---

# 🚧 Short-Term

The next releases will focus on improving flexibility and performance.

## Pipeline

- [ ] Configurable pipeline ordering
- [ ] Conditional behaviors
- [ ] Custom behavior registration

---

## Performance

- [ ] Compression behavior
- [ ] Benchmark suite
- [ ] Additional performance metrics

---

## Developer Experience

- [ ] More samples
- [ ] Extended documentation
- [ ] Analyzer package
- [ ] Source Link support

---

# 🚀 Mid-Term

The following features will expand the framework beyond Redis.

## Storage Providers

- [ ] SQL Server
- [ ] PostgreSQL
- [ ] Cosmos DB

---

## Cache Strategies

- [ ] Sliding expiration
- [ ] Cache warming
- [ ] Background preloading
- [ ] Hybrid cache strategies

---

## Distributed Features

- [ ] Distributed invalidation events
- [ ] Cache synchronization

---

# 🔮 Long-Term Vision

Future versions aim to transform the framework into a complete caching platform.

## Multi-Level Cache

- [ ] L1 Memory
- [ ] L2 Redis
- [ ] Transparent synchronization

---

## Provider SDK

- [ ] Public provider SDK
- [ ] Storage provider templates
- [ ] Community provider support

---

## Advanced Features

- [ ] Adaptive expiration
- [ ] Cache analytics
- [ ] Native AOT optimizations
- [ ] Source Generator support

---

# 💡 Future Pipeline Behaviors

The pipeline architecture makes it possible to introduce additional behaviors without modifying existing code.

Possible future behaviors include:

- [ ] CompressionBehavior
- [ ] EncryptionBehavior
- [ ] TracingBehavior
- [ ] AuditBehavior
- [ ] ValidationBehavior
- [ ] RateLimitingBehavior
- [ ] CacheWarmupBehavior
- [ ] CacheReplicationBehavior

---

# Community Ideas

Ideas proposed by the community may be incorporated into future releases.

Suggestions are welcome through:

- GitHub Issues
- GitHub Discussions
- Pull Requests

---

# Release Strategy

The project follows Semantic Versioning.

| Version | Focus |
|---------|-------|
| 1.x | Stability, bug fixes, documentation |
| 2.x | New providers and extensibility |
| 3.x | Advanced caching strategies |
| 4.x | Ecosystem integration |

---

# Contributing

If you'd like to contribute to any roadmap item, feel free to open an issue or submit a Pull Request.

Community contributions are always welcome.