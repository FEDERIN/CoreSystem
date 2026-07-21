# ⚡ CoreSystem.Resilience

> **Production-ready resilience framework for .NET 8**

CoreSystem.Resilience provides a clean abstraction over resilience strategies for .NET applications. It enables applications to define named resilience pipelines while keeping business code independent from the underlying resilience implementation.

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
- ✅ Extensible strategy builders
- ✅ Strongly typed configuration

---

## 📦 Installation

```bash
dotnet add package CoreSystem.Resilience
```

---

## 🚀 Quick Start

```csharp
builder.Services.AddResilience(options =>
{
    options.AddPipeline(PipelineType.Redis, pipeline =>
    {
        pipeline.AddRetry(retry =>
        {
            retry.MaxRetryAttempts = 3;
        });

        pipeline.AddTimeout(timeout =>
        {
            timeout.Timeout = TimeSpan.FromSeconds(2);
        });

        pipeline.AddCircuitBreaker(cb =>
        {
            cb.FailureRatio = 0.5;
        });
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
| Retry | Retries transient failures. |
| Circuit Breaker | Prevents repeated calls to unhealthy dependencies. |
| Timeout | Limits the execution time of an operation. |

---

## 📊 Built-in Metrics

CoreSystem.Resilience publishes metrics using **System.Diagnostics.Metrics**.

These metrics are compatible with **OpenTelemetry** and any OTLP-compatible backend.

| Metric | Description |
|---------|-------------|
| `core.resilience.executions` | Total pipeline executions. |
| `core.resilience.execution.duration` | Pipeline execution duration. |
| `core.resilience.retry.attempts` | Total retry attempts. |
| `core.resilience.retry.successes` | Successful executions after one or more retries. |
| `core.resilience.retry.failures` | Operations that failed after all retries. |
| `core.resilience.circuitbreaker.opened` | Circuit breaker opened events. |
| `core.resilience.circuitbreaker.closed` | Circuit breaker closed events. |
| `core.resilience.circuitbreaker.half_opened` | Circuit breaker half-open transitions. |
| `core.resilience.timeout.total` | Timeout events. |
| `core.resilience.failures` | Total failed pipeline executions. |

Register the meter:

```csharp
builder.Services
    .AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddMeter("Core.Resilience");
    });
```

### Compatible Platforms

| Platform | Supported |
|-----------|:---------:|
| Prometheus | ✅ |
| Grafana | ✅ |
| Azure Monitor | ✅ |
| OTLP | ✅ |
| Datadog | ✅ |

---

## 🏗 Architecture

```text
Application
      │
      ▼
IResiliencePipelineProvider
      │
      ▼
IResiliencePipeline
      │
      ├── Retry
      ├── Timeout
      ├── Circuit Breaker
      ├── Metrics
      │
      ▼
Protected Operation
```

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
