# ⚡ CoreSystem.Idempotency

CoreSystem.Idempotency is a production-ready idempotency framework for .NET 8.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Idempotency?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Idempotency?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-Enabled-purple?style=for-the-badge)
![Storage](https://img.shields.io/badge/Storage-Redis%20%7C%20PostgreSQL-green?style=for-the-badge)

It guarantees that critical operations are executed exactly once, even when clients retry requests because of timeouts, network failures, or duplicate submissions.

The framework provides request fingerprinting, distributed persistence, response replay, OpenTelemetry integration, and a provider-based architecture that supports multiple storage backends.

## 📦 CoreSystem Ecosystem

| Package | Responsibility |
|----------|----------------|
| **CoreSystem.Idempotency** | Idempotency middleware, request fingerprinting, response replay, and storage orchestration |
| **CoreSystem.Redis** | Redis connectivity and distributed storage infrastructure |
| **CoreSystem.Serialization** | JSON, MessagePack, and Protocol Buffers serialization |
| **CoreSystem.Http** | HTTP abstractions used by the middleware |
| **CoreSystem.Observability** *(Optional)* | Ready-to-use OpenTelemetry instrumentation, metrics, tracing, and diagnostics for CoreSystem packages |
| **CoreSystem.Observability.Abstractions** | Shared observability contracts for implementing custom instrumentation and integrations |


> Installing **CoreSystem.Idempotency** automatically installs the selected storage provider through NuGet dependencies.

> **Optional:** Install **CoreSystem.Observability** to enable built-in OpenTelemetry metrics and tracing. Install **CoreSystem.Observability.Abstractions** only if you need to build custom observability components or integrations.
---

## 📚 Table of Contents

- Getting Started
- Why CoreSystem.Idempotency?
- Architecture
- Configuration
- Storage Providers
- Fingerprinting
- Response Replay
- Observability
- Errors
- Best Practices
- Roadmap