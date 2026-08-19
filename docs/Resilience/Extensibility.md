# 🧩 Extensibility

One of the goals of **CoreSystem.Resilience** is to keep resilience concerns separated from application code.

The framework provides abstractions for executing resilience pipelines and building their internal strategy configuration while keeping the underlying Polly implementation behind the core pipeline abstraction.

This document describes the extension points that are currently available in the provided implementation.

---

# Extension Points

The current implementation provides several extension points and abstractions.

| Extension Point      | Purpose                                                            |
| -------------------- | ------------------------------------------------------------------ |
| Pipeline Builder     | Defines how resilience pipelines are built.                        |
| Pipeline Types       | Select the logical pipeline configuration to resolve.              |
| Dependency Injection | Register and replace framework services.                           |
| Metrics              | Integrate the framework metrics with observability infrastructure. |

The internal strategy builder mechanism is used by the framework to configure the implemented strategies, but it is not currently exposed as a public extension API.

---

# Pipeline Builder

The framework exposes the `IPipelineBuilder` abstraction for creating resilience pipelines.

```text
PipelineOptions

↓

IPipelineBuilder

↓

IResiliencePipeline

↓

Protected Operation
```

The default implementation is `PipelineBuilder`.

It receives the registered strategy builders, orders them, configures the Polly pipeline, and returns an `IResiliencePipeline`.

Applications can replace the registered `IPipelineBuilder` implementation through the standard Dependency Injection container when a different pipeline-building behavior is required.

---

# Strategy Builders

The framework internally uses `IStrategyBuilder` to configure resilience strategies.

The current implementation registers:

* Retry
* Timeout
* Circuit Breaker

Each strategy builder defines an execution order and configures the Polly pipeline when its corresponding options are enabled.

```text
Timeout

↓

Retry

↓

Circuit Breaker

↓

Protected Operation
```

`IStrategyBuilder` and the built-in strategy builders are internal implementation details. The provided code does not currently expose a public SDK mechanism for applications to register custom strategy builders.

---

# Pipeline Types

Applications select pipelines using the `PipelineType` abstraction.

The current predefined types are:

```text
Default
Redis
Sql
Http
Messaging
```

Each type can have its own `PipelineOptions` configuration.

For example:

```csharp
builder.Services.AddCoreResilience(options =>
{
    options.AddPipeline(PipelineType.Redis, pipeline =>
    {
        pipeline.AddRetry(retry =>
        {
            retry.MaxRetryAttempts = 3;
        });
    });
});
```

The `IResiliencePipelineProvider` resolves the configured pipeline by its `PipelineType`.

The provided implementation does not include a mechanism for dynamically adding new values to the `PipelineType` enum.

---

# Replacing Default Services

Framework services are registered through the standard .NET Dependency Injection container.

The current implementation registers `IPipelineBuilder` as a singleton.

Applications can replace framework registrations when customization of the pipeline-building process is required.

For example:

```csharp
services.AddSingleton<IPipelineBuilder, CustomPipelineBuilder>();
```

Replacing framework services should be reserved for scenarios where the default implementation does not provide the required behavior.

---

# Metrics

CoreSystem.Resilience publishes its built-in metrics through `System.Diagnostics.Metrics`.

The current implementation records:

* Retry attempts.
* Timeout events.
* Circuit breaker state transitions.
* Pipeline execution duration.

The framework also provides an observability contributor that registers the `Core.Resilience` meter with OpenTelemetry.

Applications can combine these metrics with their own application-level telemetry.

---

# Current Extensibility Boundaries

The current implementation provides extensibility primarily through abstractions and Dependency Injection.

The following capabilities are **not currently exposed as public extension mechanisms by the provided code**:

* Registering custom `IStrategyBuilder` implementations through a public API.
* Adding new `PipelineType` values dynamically.
* Registering custom resilience strategies through a dedicated strategy SDK.
* Dynamically discovering external strategy implementations.

New resilience strategies would currently require changes to the framework implementation itself.

---

# Best Practices

✅ Use `IResiliencePipeline` and `IResiliencePipelineProvider` instead of depending directly on Polly.

✅ Configure separate pipelines for different infrastructure workloads.

✅ Prefer the existing `IPipelineBuilder` abstraction before replacing framework services.

✅ Keep custom pipeline-building logic isolated from business code.

✅ Use the built-in metrics together with application-specific telemetry.

---

# Summary

CoreSystem.Resilience provides a modular architecture based on public pipeline abstractions and Dependency Injection.

The framework internally composes Retry, Timeout, and Circuit Breaker through strategy builders, while `IResiliencePipeline` keeps consumers independent from the underlying Polly pipeline.

The current implementation supports customization through the pipeline builder and Dependency Injection, but it does not yet expose a public mechanism for registering custom resilience strategies or dynamically extending `PipelineType`.
