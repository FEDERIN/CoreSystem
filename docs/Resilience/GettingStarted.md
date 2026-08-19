# 🚀 Getting Started

Welcome to **CoreSystem.Resilience**, a resilience framework for **.NET 8**.

This guide will help you configure your first resilience pipeline and execute protected operations in a few minutes.

By the end of this guide you will know how to:

- Install the package
- Register the framework
- Configure a resilience pipeline
- Execute operations through a pipeline

> **Estimated time:** 5 minutes

---

# 📋 Prerequisites

Before getting started, ensure you have:

- .NET 8 SDK
- An ASP.NET Core application (or any .NET application using Microsoft Dependency Injection)
- Basic knowledge of Dependency Injection

---

# 📦 Step 1 — Install the Package

Install the NuGet package.

```bash
dotnet add package CoreSystem.Resilience
```

---

# ⚙️ Step 2 — Register the Framework

Register **CoreSystem.Resilience** in the dependency injection container.

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

This registers a resilience pipeline containing:

- Retry
- Timeout
- Circuit Breaker

The strategies are applied in the framework-defined order:

1. Timeout
2. Retry
3. Circuit Breaker

---

# 🧩 Step 3 — Resolve a Pipeline

Inject `IResiliencePipelineProvider` into your service.

```csharp
public sealed class RedisService(
    IResiliencePipelineProvider provider)
{
    private readonly IResiliencePipeline _pipeline =
        provider.GetPipeline(PipelineType.Redis);
}
```

The provider resolves the configured pipeline by its `PipelineType`.

If the requested pipeline has not been registered, the provider throws `ResiliencePipelineNotFoundException`.

---

# ▶️ Step 4 — Execute an Operation

Protect an asynchronous operation by executing it through the pipeline.

```csharp
await _pipeline.ExecuteAsync(async cancellationToken =>
{
    await redisDatabase.StringGetAsync(
        "products:1",
        cancellationToken);
});
```

The operation receives the `CancellationToken` provided by the resilience pipeline and should observe it.

---

# 🛡 Adding Multiple Strategies

Pipelines may contain one or more resilience strategies.

Example:

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
            Timeout = TimeSpan.FromSeconds(5)
        };

        pipeline.CircuitBreaker = new CircuitBreakerOptions
        {
            FailureRatio = 0.5,
            MinimumThroughput = 10
        };
    });
});
```

Only configured and enabled strategies are added to the pipeline.

The core currently provides:

- Retry
- Timeout
- Circuit Breaker
