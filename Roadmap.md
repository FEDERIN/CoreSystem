# 🗺️ Roadmap

This document outlines the planned evolution of **CoreSystem.Idempotency**.

The project continues to evolve toward a modular, provider-based idempotency platform for ASP.NET Core applications.

Roadmap priorities may evolve based on production experience, community feedback, and ecosystem growth.

---

# Guiding Principles

CoreSystem.Idempotency evolves around the following principles:

- Production-first
- Cloud-native
- Provider-independent
- Extensible architecture
- OpenTelemetry-first
- Backward compatibility whenever possible

---

# ✅ Current Capabilities

## Core Framework

- [x] ASP.NET Core middleware
- [x] Request fingerprinting
- [x] Response replay
- [x] Configurable expiration
- [x] Configurable HTTP methods
- [x] Configurable fingerprint generation
- [x] Dependency Injection integration
- [x] Storage abstraction (`IIdempotencyStorage`)
- [x] Built-in OpenTelemetry metrics

---

## Storage Providers

- [x] CoreSystem.Idempotency.Redis
- [x] CoreSystem.Idempotency.PostgreSql

---

## Observability

- [x] Request metrics
- [x] Cache hit/miss metrics
- [x] Response replay metrics
- [x] Storage latency metrics
- [x] Payload size metrics

---

# 🚧 Near-Term Goals

The next releases focus on improving developer experience and extensibility.

## Documentation

- [ ] More end-to-end samples
- [ ] Storage provider guides
- [ ] Provider authoring guide
- [ ] Migration guides

---

## Developer Experience

- [ ] Roslyn analyzer package
- [ ] Source Link support
- [ ] XML documentation improvements
- [ ] Additional integration tests

---

## Observability

- [ ] Additional middleware metrics
- [ ] Distributed tracing
- [ ] Metric enrichment hooks

---

# 🚀 Future Providers

One of the primary goals of the architecture is enabling new storage providers without modifying the core framework.

Potential providers include:

- [ ] SQL Server
- [ ] MongoDB
- [ ] Azure Cosmos DB
- [ ] DynamoDB
- [ ] MySQL

---

# 🔌 Extensibility

Future releases will continue expanding customization points.

- [ ] Public provider SDK
- [ ] Custom fingerprint providers
- [ ] Custom fingerprint algorithms
- [ ] Custom response serializers
- [ ] Storage provider health checks

---

# ⚡ Performance

Continuous performance improvements remain a long-term objective.

- [ ] Benchmark suite
- [ ] Native AOT optimizations
- [ ] Allocation reduction
- [ ] Throughput benchmarks
- [ ] Large payload optimizations

---

# 🌍 Long-Term Vision

The long-term vision is to establish CoreSystem.Idempotency as a complete provider-based idempotency platform.

Future areas include:

## Distributed Systems

- [ ] Multi-region replication
- [ ] Cross-region consistency strategies
- [ ] Active/Active deployments
- [ ] Provider failover strategies

---

## Enterprise Features

- [ ] Administrative APIs
- [ ] Operational dashboards
- [ ] Storage diagnostics
- [ ] Retention policies

---

## CoreSystem Ecosystem

- [ ] Health Checks
- [ ] Deeper CoreSystem.Observability integration
- [ ] Source Generator support
- [ ] Additional provider packages

---

# Community Ideas

Community feedback plays an important role in shaping future releases.

Ideas and suggestions are welcome through:

- GitHub Issues
- GitHub Discussions
- Pull Requests

---

# Release Strategy

The project follows Semantic Versioning.

| Version | Focus |
|----------|-------|
| **1.x** | Stability, documentation, provider maturity |
| **2.x** | Additional providers and extensibility |
| **3.x** | Distributed capabilities and enterprise features |

---

# Contributing

Contributions of any size are welcome.

Whether you're fixing bugs, improving documentation, implementing a storage provider, or proposing new ideas, community participation helps shape the future of CoreSystem.Idempotency.