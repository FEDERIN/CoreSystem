# 🔄 CoreSystem.Cache.Rehydration

> **Cache recovery component for CoreSystem.Cache on .NET 8**

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Cache.Rehydration?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Cache.Rehydration?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

`CoreSystem.Cache.Rehydration` restores cache entries that were temporarily
stored in the memory fallback after the primary cache provider becomes healthy
again.

The package is designed to work with `CoreSystem.Cache` and an external primary
cache provider.

---

## 📦 CoreSystem Ecosystem

| Package | Responsibility |
|----------|----------------|
| **CoreSystem.Cache** | Cache orchestration and in-memory fallback |

`CoreSystem.Cache.Rehydration` does not implement a cache provider.

It reads tracked entries from the memory fallback and writes them to the
`ICacheStorageResolver.Primary` storage while preserving remaining expiration
and tags.

An external cache provider must be registered before rehydration is enabled.

---

## 📚 Table of Contents

- 🚀 Getting Started
- 🏗️ Architecture
- ⚙️ Configuration
- 🧑‍💻 Basic Usage
- 🗺️ Roadmap