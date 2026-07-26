# Best Practices

This guide describes the recommended practices for building applications with the CoreSystem ecosystem.

Following these guidelines helps you create applications that are more reliable, maintainable, observable, and performant.

---

# Design Principles

CoreSystem is built around a few fundamental principles.

## Keep packages focused

Each package should solve a single problem.

Good

- CoreSystem.Cache
- CoreSystem.Resilience
- CoreSystem.Http

Avoid creating packages with multiple unrelated responsibilities.

---

## Prefer composition over inheritance

Compose behaviors using Dependency Injection instead of inheritance whenever possible.

Example

```csharp
builder.Services
    .AddCoreCache()
    .AddCoreResilience()
    .AddCoreHttp();
```

---

## Follow dependency inversion

Depend on abstractions instead of concrete implementations.

Good

```csharp
ICoreCache
```

Avoid

```csharp
RedisCache
```

---

# Dependency Injection

Register packages using the provided extension methods.

```csharp
builder.Services.AddCoreCache();
builder.Services.AddCoreResilience();
```

Avoid manual service registration unless customization is required.

---

# Configuration

Keep configuration inside appsettings.json.

Good

```json
{
  "Core": {
    "Cache": {
      "DefaultExpiration": "00:30:00"
    }
  }
}
```

Avoid hardcoded values.

---

# Caching

Use Cache-Aside whenever possible.

✔ Cache reads

✔ Invalidate after writes

✔ Cache immutable data

Avoid caching rapidly changing information unless necessary.

---

# Resilience

Use Retry only for transient failures.

Use Timeout to avoid indefinitely waiting operations.

Use Circuit Breaker to prevent cascading failures.

Avoid retrying validation errors or business rule violations.

---

# Observability

Enable metrics and tracing in production.

Expose OpenTelemetry metrics whenever possible.

Monitor

- Cache Hit Rate
- Cache Miss Rate
- Retry Count
- Timeout Count
- Circuit Breaker State

---

# Performance

Reuse services registered by Dependency Injection.

Avoid unnecessary serialization.

Prefer asynchronous APIs.

Avoid blocking calls.

---

# Error Handling

Log unexpected failures.

Do not swallow exceptions.

Throw meaningful exceptions.

---

# Testing

Unit test public APIs.

Use integration tests for infrastructure components.

Validate configuration during startup.

---

# Security

Never cache sensitive information without encryption.

Validate external inputs.

Protect configuration secrets.

---

# Versioning

Follow Semantic Versioning.

Increment

- MAJOR for breaking changes
- MINOR for new features
- PATCH for fixes

---

# Documentation

Document every public API.

Provide code examples.

Keep documentation synchronized with releases.

---

# Summary

Following these practices helps build applications that are

- Reliable
- Observable
- Performant
- Maintainable
- Easy to evolve