# 🗺️ Roadmap

This document outlines the planned evolution of
**CoreSystem.Cache.Rehydration**.

The roadmap focuses on the recovery component itself and does not claim
features implemented by external cache providers.

---

# ✅ Current Capabilities

The current implementation provides:

- [x] Memory fallback entry tracking
- [x] Primary cache recovery detection
- [x] Rehydration of tracked entries
- [x] Remaining expiration preservation
- [x] Tag preservation
- [x] Batch processing
- [x] Background rehydration cycles
- [x] Retry on later cycles after individual entry failures

---

# 🚧 Future

Potential future work may include:

- [ ] Additional rehydration diagnostics
- [ ] More configurable batch processing
- [ ] Additional recovery policies

---

# 🔮 Long-Term

Future versions may expand the recovery component while keeping the source and
primary-target responsibilities separate from cache provider implementations.
