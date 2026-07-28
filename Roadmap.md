# 🗺️ Roadmap

This document outlines the planned evolution of **CoreSystem.Idempotency**.

The roadmap provides visibility into the long-term direction of the project. Priorities may evolve based on community feedback, production experience, and ecosystem growth.

---

# Guiding Principles

CoreSystem.Idempotency will continue to evolve following these principles:

- Production-first
- Cloud-native
- Storage-provider independent
- Extensible architecture
- OpenTelemetry-first
- Backward compatibility whenever possible

---

# ✅ Completed

The following capabilities are already available.

## Core Framework

- [x] ASP.NET Core middleware
- [x] Dependency Injection integration
- [x] Configurable middleware options
- [x] Response replay
- [x] Request fingerprinting
- [x] Configurable idempotency header
- [x] Configurable expiration
- [x] Configurable HTTP methods

---

## Storage Providers

- [x] Redis provider
- [x] PostgreSQL provider
- [x] Storage abstraction

---

## Request Processing

- [x] Request fingerprint validation
- [x] Duplicate request detection
- [x] Response persistence
- [x] Configurable cacheable status codes

---

## Observability

- [x] OpenTelemetry Metrics
- [x] Storage latency metrics
- [x] Payload size metrics

---

# 🚧 Short-Term

The next releases focus on improving flexibility and developer experience.

## Configuration

- [ ] Additional fingerprint configuration
- [ ] Custom hash algorithms
- [ ] Improved configuration validation

---

## Developer Experience

- [ ] More samples
- [ ] Expanded documentation
- [ ] Analyzer package
- [ ] Source Link support

---

## Observability

- [ ] Additional metrics
- [ ] Distributed tracing
- [ ] Custom metric enrichment

---

# 🚀 Mid-Term

Future releases will expand provider support and extensibility.

## Storage Providers

- [ ] SQL Server
- [ ] MongoDB
- [ ] Azure Cosmos DB

---

## Extensibility

- [ ] Public storage provider SDK
- [ ] Custom fingerprint providers
- [ ] Custom response serializers

---

## Performance

- [ ] Benchmark suite
- [ ] Native AOT optimizations
- [ ] Allocation reduction

---

# 🔮 Long-Term Vision

Future versions aim to provide a complete distributed idempotency platform.

## Distributed Features

- [ ] Multi-region replication support
- [ ] Cross-region consistency strategies
- [ ] Provider failover

---

## Enterprise Features

- [ ] Administrative APIs
- [ ] Storage diagnostics
- [ ] Operational dashboards

---

## Ecosystem Integration

- [ ] Deeper CoreSystem.Observability integration
- [ ] Health Checks
- [ ] Source Generator support

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
| **1.x** | Stability, documentation, production adoption |
| **2.x** | Additional storage providers and extensibility |
| **3.x** | Enterprise capabilities and distributed features |

---

# Contributing

If you'd like to contribute to any roadmap item, feel free to open an issue or submit a Pull Request.

Community contributions are always welcome.