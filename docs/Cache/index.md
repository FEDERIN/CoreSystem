# ⚡ CoreSystem.Cache

> **Production-ready distributed caching framework for .NET 8**

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Cache?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Cache?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-Enabled-purple?style=for-the-badge)

CoreSystem.Cache is a production-ready distributed caching framework for .NET 8.

It provides a high-performance cache execution pipeline with Cache-Aside support, HTTP response caching, resilience, health checks, and OpenTelemetry integration.

The framework is built on top of a modular ecosystem of specialized packages for cache providers and serialization.

## 📦 CoreSystem Ecosystem

| Package | Responsibility |
|----------|----------------|
| **CoreSystem.Cache** | Cache orchestration, execution pipeline, HTTP response caching, Cache-Aside, observability, and resilience |
| **CoreSystem.Cache.Memory** | In-memory cache provider |
| **CoreSystem.Cache.Redis** | Redis cache provider |
| **CoreSystem.Serialization** | JSON, MessagePack, and Protocol Buffers serialization |

> Installing **CoreSystem.Cache** automatically installs the required provider and serialization packages through NuGet dependencies.

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
- Extensibility
- Roadmap