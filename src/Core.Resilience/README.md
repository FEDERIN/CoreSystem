# ⚡ CoreSystem.Resilience

> **Production-ready resilience framework for .NET 8**

CoreSystem.Resilience provides a clean abstraction over resilience strategies for .NET applications. It enables applications to define named resilience pipelines while keeping application code independent from the underlying Polly implementation.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Resilience?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Resilience?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

---

## ✨ Features

- ✅ Retry strategy
- ✅ Circuit Breaker strategy
- ✅ Timeout strategy
- ✅ Named resilience pipelines
- ✅ Dependency Injection integration
- ✅ Polly abstraction
- ✅ Built-in metrics
- ✅ OpenTelemetry compatible
- ✅ Configurable handled exceptions
- ✅ Strongly typed configuration

---

## 📦 Installation

```bash
dotnet add package CoreSystem.Resilience
```

---

## 🚀 Quick Start

Register the framework:

```csharp
builder.Services.AddCoreResilience(options =>
{
    options.AddPipeline(PipelineType.Redis, pipeline =>
    {
        pipeline.Retry = new RetryOptions
        {
            MaxRetryAttempts = 3
        };

        pipeline.Timeout = new TimeoutOptions
        {
            Timeout = TimeSpan.FromSeconds(2)
        };

        pipeline.CircuitBreaker = new CircuitBreakerOptions
        {
            FailureRatio = 0.5
        };
    });
});
```

Resolve a pipeline:

```csharp
public sealed class RedisService(
    IResiliencePipelineProvider provider)
{
    private readonly IResiliencePipeline _pipeline =
        provider.GetPipeline(PipelineType.Redis);
}
```

Execute an operation:

```csharp
await _pipeline.ExecuteAsync(async ct =>
{
    await redis.GetAsync(key, ct);
});
```

---

## 🛡 Supported Strategies

| Strategy | Description |
|----------|-------------|
| Retry | Retries handled exceptions according to the configured retry options. |
| Circuit Breaker | Opens the circuit when the configured failure conditions are reached. |
| Timeout | Limits the execution time of an operation. |

The execution pipeline applies configured strategies in the following order:

```text
Timeout → Retry → Circuit Breaker
```

Only strategies configured for a pipeline are added to its execution pipeline.

---

## 📊 Built-in Metrics

CoreSystem.Resilience publishes metrics using **System.Diagnostics.Metrics**.

The `Core.Resilience` meter is registered for OpenTelemetry integration.

| Metric | Description |
|---------|-------------|
| `core.resilience.retry.attempts` | Total retry attempts executed by the retry strategy. |
| `core.resilience.timeout.triggered` | Total operations that exceeded the configured timeout. |
| `core.resilience.circuit.opened` | Total circuit breaker transitions to Open. |
| `core.resilience.circuit.half_opened` | Total circuit breaker transitions to Half-Open. |
| `core.resilience.circuit.closed` | Total circuit breaker transitions to Closed. |
| `core.resilience.pipeline.duration` | Execution time of the resilience pipeline. |

Register the meter with OpenTelemetry:

```csharp
builder.Services
    .AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddMeter("Core.Resilience");
    });
```

---

## 🏗 Architecture

```text
Application
      │
      ▼
IResiliencePipelineProvider
      │
      ▼
PipelineRegistry
      │
      ▼
IResiliencePipeline
      │
      ├── Timeout
      ├── Retry
      ├── Circuit Breaker
      │
      ▼
Polly ResiliencePipeline
      │
      ▼
Protected Operation
```

Each configured `PipelineType` is registered in the pipeline registry and resolved through `IResiliencePipelineProvider`. The public abstraction exposes pipeline execution without requiring application code to depend directly on Polly.

---

## 📖 Documentation

The full documentation includes:

- Getting Started
- Configuration
- Retry
- Circuit Breaker
- Timeout
- Metrics
- Architecture
- Extensibility

Visit the GitHub repository for the complete documentation.

---

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

---

## 📄 License

Released under the MIT License.
