# 🗺️ Roadmap

This document outlines the planned evolution of **CoreSystem.Cache.Redis**.

The roadmap focuses on the Redis provider while recognizing the optional
resilience and rehydration components of the CoreSystem cache ecosystem.

---

# ✅ Current Capabilities

The current implementation provides:

- [x] Redis external cache storage
- [x] Redis connection registration
- [x] Cache serialization
- [x] Cache-Aside through `GetOrAddAsync`
- [x] Distributed locking for `GetOrAddAsync`
- [x] Tag-based invalidation
- [x] Redis health checks
- [x] Primary health state integration
- [x] Redis-specific resilience integration
- [x] Compatibility with `CoreSystem.Cache.Rehydration`

---

# 🚧 Future

Potential future work may include:

- [ ] Additional Redis-specific performance optimizations
- [ ] Expanded Redis diagnostics
- [ ] Additional provider configuration options
- [ ] Extended Redis integration tests

---

# 🔮 Long-Term

Future versions may expand the Redis provider while keeping the provider
boundary defined by `CoreSystem.Cache`.

Possible areas include:

- [ ] Advanced Redis caching strategies
- [ ] Additional Redis operational capabilities
- [ ] Broader provider-level diagnostics
