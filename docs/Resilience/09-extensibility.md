# 🧩 Extensibility

One of the primary goals of **CoreSystem.Resilience** is to provide a resilience framework that can evolve without requiring changes to application code.

Rather than coupling resilience policies directly to business logic, the framework exposes a set of extension points that allow new resilience strategies, pipeline builders, metrics, and pipeline types to be introduced independently.

This document describes the available extension points and the recommended approach for extending the framework.

---

# Extension Points

The framework can be extended in several ways.

| Extension Point      | Purpose                             |
| -------------------- | ----------------------------------- |
| Pipeline Builder     | Build custom resilience pipelines   |
| Strategy Builders    | Add new resilience strategies       |
| Pipeline Types       | Register additional named pipelines |
| Metrics              | Publish custom telemetry            |
| Dependency Injection | Replace framework services          |

---

# Extending the Pipeline Builder

Every resilience pipeline is created through the `IPipelineBuilder` abstraction.

```text
Application

↓

IPipelineBuilder

↓

Resilience Pipeline

↓

Protected Operation
```

Because the builder is responsible for constructing pipelines, new strategies can be introduced without modifying existing application code.

Examples include:

* Bulkhead Isolation
* Hedging
* Rate Limiting
* Fallback
* Custom Strategies

---

# Creating a Custom Strategy

Custom strategies should encapsulate a single resilience concern.

For example, a custom builder could configure a Bulkhead strategy.

```csharp
public sealed class BulkheadStrategyBuilder
{
    public void Configure(
        ResiliencePipelineBuilder builder)
    {
        // Configure the strategy
    }
}
```

The strategy can then be integrated into the pipeline construction process.

Keeping strategies isolated makes them easier to test, maintain, and evolve independently.

---

# Registering Additional Pipeline Types

Applications may define multiple independent resilience pipelines.

For example:

```text
Redis

Http

Database

Messaging
```

Each pipeline can be configured with its own resilience policies while sharing the same programming model.

This allows different workloads to apply resilience strategies appropriate to their operational characteristics.

---

# Replacing Default Services

All framework services are registered through the standard .NET dependency injection container.

Applications can replace framework implementations whenever customization is required.

Example:

```csharp
services.Replace(
    ServiceDescriptor.Singleton<
        IPipelineBuilder,
        CustomPipelineBuilder>());
```

Replacing services should be reserved for advanced scenarios where the default implementation does not meet application requirements.

---

# Adding Custom Metrics

CoreSystem.Resilience publishes metrics through `System.Diagnostics.Metrics`.

Applications can extend observability by recording additional metrics around pipeline execution.

Examples include:

* Business-specific counters
* Tenant metrics
* SLA measurements
* Dependency-specific telemetry
* Performance indicators

Custom metrics should complement the built-in metrics while preserving consistent naming conventions.

---

# Future Extension Points

The architecture has been designed to accommodate additional resilience strategies without breaking the public API.

Potential future extensions include:

* Bulkhead Strategy
* Hedging Strategy
* Fallback Strategy
* Rate Limiter Strategy
* Chaos Engineering Strategy
* Adaptive Retry Strategy

Because pipelines are built compositionally, new strategies can be introduced independently of existing ones.

---

# Best Practices

✅ Create one strategy per resilience concern.

✅ Prefer extending the pipeline over replacing framework services.

✅ Keep custom strategies stateless whenever possible.

✅ Avoid coupling resilience logic to business code.

✅ Reuse existing abstractions before introducing new extension points.

✅ Use meaningful names for custom pipeline types.

---

# Summary

CoreSystem.Resilience is designed to evolve through composition rather than modification.

By exposing well-defined extension points, the framework allows applications to introduce new resilience capabilities while preserving a stable public API and maintaining a clean separation between infrastructure and business logic.
