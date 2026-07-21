# 🗺️ Roadmap

This document outlines the long-term vision for **CoreSystem.Resilience**.

The roadmap provides visibility into the planned evolution of the framework. Priorities may change based on community feedback, real-world adoption, and the evolution of the .NET ecosystem.

---

# Guiding Principles

CoreSystem.Resilience will continue to evolve following these principles:

- Production-first
- Cloud-native
- Extensible architecture
- OpenTelemetry-first
- Dependency Injection
- Backward compatibility whenever possible

---

# ✅ Completed

The following capabilities are already available.

## Core Infrastructure

- [x] Named resilience pipelines
- [x] Dependency Injection integration
- [x] Pipeline provider abstraction
- [x] Strongly typed configuration
- [x] Pipeline registry

---

## Resilience Strategies

- [x] Retry
- [x] Circuit Breaker
- [x] Timeout

---

## Configuration

- [x] Fluent configuration API
- [x] appsettings.json support
- [x] Strongly typed options

---

## Observability

- [x] Built-in metrics
- [x] OpenTelemetry integration
- [x] Execution duration metrics
- [x] Retry metrics
- [x] Timeout metrics
- [x] Circuit Breaker metrics

---

# 🚧 Short-Term

The next releases will focus on improving flexibility and developer experience.

## Pipeline

- [ ] Conditional strategy execution
- [ ] Pipeline validation
- [ ] Strategy ordering customization
- [ ] Named pipeline conventions

---

## Metrics

- [ ] Additional resilience metrics
- [ ] Metric tags and dimensions
- [ ] Pipeline-specific metrics

---

## Developer Experience

- [ ] More samples
- [ ] Extended documentation
- [ ] XML documentation improvements
- [ ] Source Link support

---

# 🚀 Mid-Term

The following features will improve extensibility and operational capabilities.

## Resilience Strategies

- [ ] Rate Limiter
- [ ] Hedging
- [ ] Fallback
- [ ] Bulkhead Isolation

---

## Configuration

- [ ] Dynamic configuration reload
- [ ] Environment-specific configuration
- [ ] Configuration validation

---

## Observability

- [ ] Distributed tracing
- [ ] Activity correlation
- [ ] Diagnostic events

---

# 🔮 Long-Term Vision

Future versions aim to transform CoreSystem.Resilience into a complete resilience platform for .NET applications.

## Advanced Pipeline Features

- [ ] Pipeline composition
- [ ] Nested pipelines
- [ ] Pipeline templates
- [ ] Shared strategy profiles

---

## Runtime Features

- [ ] Dynamic pipeline registration
- [ ] Runtime pipeline discovery
- [ ] Pipeline diagnostics dashboard

---

## Ecosystem Integration

- [ ] HTTP client integration
- [ ] Messaging integration
- [ ] Background services integration
- [ ] Database provider integration

---

# 💡 Future Strategies

The pipeline architecture allows new resilience strategies to be introduced without breaking the public API.

Potential future strategies include:

- [ ] Fallback Strategy
- [ ] Bulkhead Strategy
- [ ] Hedging Strategy
- [ ] Rate Limiter Strategy
- [ ] Chaos Engineering Strategy
- [ ] Adaptive Retry Strategy

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
| **1.x** | Stability, bug fixes, documentation |
| **2.x** | Additional resilience strategies |
| **3.x** | Advanced pipeline composition |
| **4.x** | Complete ecosystem integration |

---

# Contributing

Contributions are always welcome.

Whether you're fixing bugs, improving documentation, proposing new resilience strategies, or enhancing the developer experience, your feedback helps make the project better for everyone.

If you'd like to contribute:

- Open a GitHub Issue
- Start a GitHub Discussion
- Submit a Pull Request

Thank you for helping improve **CoreSystem.Resilience**.