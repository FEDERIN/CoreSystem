# ⚡ CoreSystem.Cache.Redis

> **Redis provider for CoreSystem.Cache on .NET 8**

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Cache.Redis?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Cache.Redis?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

`CoreSystem.Cache.Redis` provides the Redis storage implementation used by
`CoreSystem.Cache`.

It integrates Redis as the external cache storage and supports cache operations,
tag-based invalidation, distributed locking, health checks, and integration with
the resilience and rehydration components of the CoreSystem cache ecosystem.

---

## 📦 CoreSystem Ecosystem

| Package | Responsibility |
|----------|----------------|
| **CoreSystem.Redis** | Redis connectivity infrastructure used by the Redis provider |
| **CoreSystem.Cache** | Cache orchestration and in-memory fallback |
| **CoreSystem.Cache.Rehydration** | Fallback entry restoration |

---

## 📚 Table of Contents

- 🚀 Getting Started
- 🏗️ Architecture
- ⚙️ Configuration
- 🧑‍💻 Basic Usage
- ❤️ Health Checks
- 🧩 Extensibility
- 🗺️ Roadmap