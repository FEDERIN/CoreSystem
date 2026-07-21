# 🚀 Getting Started

Welcome to **CoreSystem.Resilience**, a production-ready resilience framework for **.NET 8**.

This guide will help you configure your first resilience pipeline and execute protected operations in just a few minutes.

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

        pipeline.AddCircuitBreaker(circuit =>
        {
            circuit.FailureRatio = 0.5;
        });
    });
});
```

This registers a named resilience pipeline containing:

- Retry
- Timeout
- Circuit Breaker

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

The provider resolves the configured pipeline by its type.

---

# ▶️ Step 4 — Execute an Operation

Protect any asynchronous operation by executing it through the pipeline.

```csharp
await _pipeline.ExecuteAsync(async cancellationToken =>
{
    await redisDatabase.StringGetAsync("products:1");
});
```

The framework automatically applies every configured resilience strategy before executing the operation.

---

# 🛡 Adding Multiple Strategies

Pipelines may contain one or more resilience strategies.

Example:

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
            timeout.Timeout = TimeSpan.FromSeconds(5);
        });

        pipeline.AddCircuitBreaker(circuit =>
        {
            circuit.FailureRatio = 0.5;
            circuit.MinimumThroughput = 10;
        });
    });
});
```

Strategies execute in the order they are registered.