# 🏗️ Architecture

`CoreSystem.Resilience` is built around the concept of **named resilience pipelines**.

Rather than coupling resilience strategies directly to application code, every protected operation is executed through an `IResiliencePipeline`. Each pipeline is composed of one or more resilience strategies, allowing infrastructure concerns to remain isolated from business logic.

The framework provides a lightweight abstraction over Polly while exposing a clean, provider-independent API that can evolve without affecting consuming applications.

---

# Architectural Overview

Applications interact only with the public abstractions exposed by the framework.

The `IResiliencePipelineProvider` resolves a named pipeline, which executes the configured resilience strategies before invoking the protected operation.

```mermaid
graph TD

    App["Application"]

    App --> Provider["IResiliencePipelineProvider"]

    Provider --> Pipeline["IResiliencePipeline"]

    Pipeline --> Retry["Retry Strategy"]

    Retry --> Timeout["Timeout Strategy"]

    Timeout --> Circuit["Circuit Breaker Strategy"]

    Circuit --> Execute["Protected Operation"]
```

This architecture separates business logic from resilience concerns while allowing new strategies to be introduced without changing application code.

---

# Design Goals

The framework is designed around a few core principles.

- Keep business code independent from resilience implementations.
- Hide Polly behind a stable abstraction.
- Support multiple named resilience pipelines.
- Allow strategies to evolve without affecting consumers.
- Integrate naturally with Dependency Injection.
- Publish operational metrics using `System.Diagnostics.Metrics`.

---

# Architectural Patterns

`CoreSystem.Resilience` combines several well-established software design patterns.

| Pattern | Purpose |
|----------|---------|
| **Builder** | Builds resilience pipelines from configuration. |
| **Strategy** | Encapsulates retry, timeout, and circuit breaker behaviors. |
| **Registry** | Stores named resilience pipelines. |
| **Provider** | Resolves pipelines by their logical type. |
| **Factory** | Creates Polly pipelines from framework configuration. |
| **Decorator** | Collects metrics around pipeline execution. |

These patterns keep the public API small while allowing the internal implementation to evolve independently.

---

# Core Components

The framework is composed of a small number of components, each with a single responsibility.

| Component | Responsibility |
|-----------|----------------|
| **IResiliencePipeline** | Executes protected operations through the configured resilience strategies. |
| **IResiliencePipelineProvider** | Resolves resilience pipelines by their logical type. |
| **PipelineBuilder** | Builds Polly pipelines from framework options. |
| **PipelineRegistry** | Stores all registered pipelines. |
| **Retry Strategy Builder** | Configures retry policies. |
| **Timeout Strategy Builder** | Configures timeout policies. |
| **Circuit Breaker Strategy Builder** | Configures circuit breaker policies. |
| **ResilienceMetrics** | Publishes execution metrics using `System.Diagnostics.Metrics`. |

---

# Pipeline Construction

During application startup, the framework builds every configured pipeline.

Each strategy contributes its configuration to the final pipeline before it is registered.

```mermaid
graph LR

    Options["ResilienceOptions"]

    Options --> Builder["PipelineBuilder"]

    Builder --> Retry["Retry"]

    Builder --> Timeout["Timeout"]

    Builder --> Circuit["Circuit Breaker"]

    Circuit --> Polly["Polly Resilience Pipeline"]

    Polly --> Registry["Pipeline Registry"]
```

Once registration completes, pipelines become immutable and can be safely reused throughout the application's lifetime.

---

# Execution Lifecycle

Every protected operation follows the same execution flow.

The application resolves a pipeline, which executes each configured resilience strategy before invoking the protected operation.

```mermaid
sequenceDiagram

    actor Client

    participant Provider as IResiliencePipelineProvider
    participant Pipeline as IResiliencePipeline
    participant Retry
    participant Timeout
    participant CircuitBreaker
    participant Operation

    Client->>Provider: GetPipeline()
    Provider-->>Client: Pipeline

    Client->>Pipeline: ExecuteAsync()

    Pipeline->>Retry: Execute
    Retry->>Timeout: Continue
    Timeout->>CircuitBreaker: Continue
    CircuitBreaker->>Operation: Execute()

    Operation-->>CircuitBreaker: Result
    CircuitBreaker-->>Timeout: Result
    Timeout-->>Retry: Result
    Retry-->>Pipeline: Result
    Pipeline-->>Client: Result
```

Each strategy participates transparently in the execution without requiring changes to application code.

---

# Metrics Flow

Every pipeline execution can publish operational metrics.

Metrics are emitted using `System.Diagnostics.Metrics` and can be collected by any OpenTelemetry-compatible exporter.

```mermaid
graph LR

    Operation["Protected Operation"]

    Operation --> Pipeline["Resilience Pipeline"]

    Pipeline --> Metrics["Resilience Metrics"]

    Metrics --> SDM["System.Diagnostics.Metrics"]

    SDM --> OTel["OpenTelemetry"]

    OTel --> Backend["Monitoring Platform"]
```

Typical monitoring platforms include:

- Prometheus
- Grafana
- Azure Monitor
- Datadog
- OTLP-compatible collectors

---

# Dependency Injection

The framework integrates with the standard ASP.NET Core Dependency Injection container.

```mermaid
graph TD

    Registration["AddResilience()"]

    Registration --> Options["ResilienceOptions"]

    Registration --> Builder["PipelineBuilder"]

    Builder --> Registry["PipelineRegistry"]

    Registry --> Provider["IResiliencePipelineProvider"]

    Provider --> Application["Application Services"]
```

Applications only depend on the public abstractions exposed by the framework.

---

# Design Principles

When extending the framework, follow these principles.

* Single Responsibility Principle
* Composition over inheritance
* Keep strategies independent
* Prefer abstractions over concrete implementations
* Integrate through dependency injection
* Preserve asynchronous execution
* Maintain provider independence

Following these principles helps ensure that custom extensions remain consistent with the framework architecture.

---

# Summary

`CoreSystem.Resilience` provides a modular architecture for executing protected operations through reusable resilience pipelines.

By combining named pipelines, configurable strategies, dependency injection, and built-in metrics, the framework delivers a clean and extensible resilience layer while keeping application code focused on business logic.

---

# 🔀 Pipelines

A resilience pipeline represents a reusable execution flow that applies one or more resilience strategies before executing an operation.

Instead of configuring resilience for every individual operation, **CoreSystem.Resilience** allows applications to register named pipelines once and reuse them throughout the application.

---

# Why Pipelines?

Modern applications interact with multiple external resources, each with different resilience requirements.

For example:

- Redis may require retries and short timeouts.
- HTTP services may require retries with exponential backoff.
- Database operations may require longer timeouts but no retries.
- Background jobs may use completely different policies.

By registering multiple pipelines, each workload can have its own resilience configuration while sharing the same programming model.

---

# Pipeline Lifecycle

The following diagram illustrates how a pipeline is created and executed.

```mermaid
flowchart TD

    Options["ResilienceOptions"]

    Options
        --> Builder["PipelineBuilder"]

    Builder
        --> Retry["Retry Strategy"]

    Retry
        --> Timeout["Timeout Strategy"]

    Timeout
        --> Circuit["Circuit Breaker Strategy"]

    Circuit
        --> Polly["Build Polly Pipeline"]

    Polly
        --> Registry["Pipeline Registry"]

    Registry
        --> Provider["IResiliencePipelineProvider"]

    Provider
        --> Application["Application"]
```

---

# Executing a Pipeline

Once resolved, the pipeline protects any asynchronous operation.

```csharp
await _pipeline.ExecuteAsync(async cancellationToken =>
{
    await redis.GetAsync(key, cancellationToken);
});
```

Every configured resilience strategy is applied automatically before the operation is executed.

---

# Execution Flow

Every execution follows the same lifecycle.

```mermaid
sequenceDiagram

    actor Client

    participant Provider
    participant Pipeline
    participant Retry
    participant Timeout
    participant CircuitBreaker
    participant Operation

    Client->>Provider: GetPipeline()
    Provider-->>Client: Pipeline

    Client->>Pipeline: ExecuteAsync()

    Pipeline->>Retry: Execute
    Retry->>Timeout: Execute
    Timeout->>CircuitBreaker: Execute
    CircuitBreaker->>Operation: Execute()

    Operation-->>CircuitBreaker: Result
    CircuitBreaker-->>Timeout: Result
    Timeout-->>Retry: Result
    Retry-->>Pipeline: Result
    Pipeline-->>Client: Result
```

---

# Combining Strategies

A pipeline may contain one or multiple resilience strategies.

For example:

| Strategy | Purpose |
|----------|----------|
| Retry | Retries transient failures. |
| Timeout | Prevents excessively long operations. |
| Circuit Breaker | Stops sending requests to unhealthy dependencies. |

Strategies are executed in the order they are registered.

---

# Multiple Pipelines

Applications can register multiple independent pipelines.

```csharp
builder.Services.AddResilience(options =>
{
    options.AddPipeline(PipelineType.Redis, pipeline =>
    {
        pipeline.AddRetry(retry =>
        {
            retry.MaxRetryAttempts = 3;
        });
    });

    options.AddPipeline(PipelineType.Http, pipeline =>
    {
        pipeline.AddRetry(retry =>
        {
            retry.MaxRetryAttempts = 5;
        });

        pipeline.AddTimeout(timeout =>
        {
            timeout.Timeout = TimeSpan.FromSeconds(30);
        });
    });
});
```

Each pipeline is completely independent from the others.

---

# Best Practices

- Register one pipeline for each infrastructure dependency.
- Reuse pipelines instead of creating them dynamically.
- Keep pipeline configurations focused on a single workload.
- Combine strategies only when they provide additional value.
- Prefer centralized registration during application startup.