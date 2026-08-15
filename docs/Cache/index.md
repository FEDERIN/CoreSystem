# ⚡ CoreSystem.Cache

> **Production-ready distributed caching framework for .NET 8**

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Cache?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Cache?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-Enabled-purple?style=for-the-badge)

CoreSystem.Cache is a distributed caching framework for .NET 8.

It provides a unified cache API with Cache-Aside support, HTTP response caching, resilience, fallback storage, health checks, and OpenTelemetry metrics.

The framework is built around a pipeline architecture that separates cache operations from storage providers and cross-cutting behaviors.

## 📦 CoreSystem Ecosystem

| Package | Responsibility |
|----------|----------------|
| **CoreSystem.Memory** | In-memory cache provider |
| **CoreSystem.Serialization** | JSON, MessagePack, and Protocol Buffers serialization |
| **CoreSystem.Http** | HTTP abstractions used by the middleware |
| **CoreSystem.Observability** *(Optional)* | Ready-to-use OpenTelemetry instrumentation, metrics, tracing, and diagnostics for CoreSystem packages |
| **CoreSystem.Observability.Abstractions** | Shared observability contracts for implementing custom instrumentation and integrations |


> Installing **CoreSystem.Cache** automatically installs the required provider and serialization packages through NuGet dependencies.

> **Optional:** Install **CoreSystem.Observability** to enable built-in OpenTelemetry metrics and tracing. Install **CoreSystem.Observability.Abstractions** only if you need to build custom observability components or integrations.

> **CoreSystem.Cache** can operate with the in-memory provider without requiring an external cache provider.

> When an external provider such as Redis is configured, additional CoreSystem packages can provide Redis storage, resilience, and cache rehydration capabilities.
---

## 📚 Table of Contents

- Getting Started
- Why CoreSystem.Cache?
- Architecture
- Configuration
- Basic Usage
- HTTP Response Caching
- Observability
- Health Checks
- Roadmap