# ⚙️ Configuration

This guide explains how to configure **CoreSystem.Resilience**.

You'll learn how to:

* Register the framework
* Configure resilience pipelines
* Configure Retry, Timeout, and Circuit Breaker strategies
* Register multiple pipelines
* Configure the framework using `appsettings.json`
* Follow recommended production settings

---

# Configuration Overview

The framework is configured through the `AddResilience()` extension.

```csharp
builder.Services.AddResilience(options =>
{
    // Configure your resilience pipelines here.
});
```

A resilience pipeline is identified by a `PipelineType` and can contain one or more resilience strategies.

---

# Creating a Pipeline

Register a new pipeline by specifying its type and configuring the desired strategies.

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

        pipeline.AddCircuitBreaker(circuitBreaker =>
        {
            circuitBreaker.FailureRatio = 0.5;
        });
    });
});
```

The framework builds the pipeline during application startup and registers it for dependency injection.

---

# Registering Multiple Pipelines

Applications can register multiple independent resilience pipelines.

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
        pipeline.AddTimeout(timeout =>
        {
            timeout.Timeout = TimeSpan.FromSeconds(10);
        });
    });
});
```

Each pipeline can be resolved independently through `IResiliencePipelineProvider`.

---

# Retry Configuration

Configure retry behavior for transient failures.

```csharp
pipeline.AddRetry(retry =>
{
    retry.Enabled = true;

    retry.MaxRetryAttempts = 3;

    retry.Delay = TimeSpan.FromMilliseconds(500);

    retry.BackoffType = BackoffType.Exponential;

    retry.UseJitter = true;
});
```

## Configuration Options

| Option           | Description                      | Default     |
| ---------------- | -------------------------------- | ----------- |
| Enabled          | Enables or disables the strategy | `true`      |
| MaxRetryAttempts | Maximum retry attempts           | `3`         |
| Delay            | Initial retry delay              | `500 ms`    |
| BackoffType      | Delay calculation strategy       | Exponential |
| UseJitter        | Randomizes retry delays          | `true`      |

---

# Timeout Configuration

Configure the maximum execution time allowed for protected operations.

```csharp
pipeline.AddTimeout(timeout =>
{
    timeout.Enabled = true;

    timeout.Timeout = TimeSpan.FromSeconds(5);
});
```

## Configuration Options

| Option  | Description                      | Default      |
| ------- | -------------------------------- | ------------ |
| Enabled | Enables or disables the strategy | `true`       |
| Timeout | Maximum execution time           | `30 seconds` |

---

# Circuit Breaker Configuration

Protect downstream services by temporarily blocking requests after repeated failures.

```csharp
pipeline.AddCircuitBreaker(circuitBreaker =>
{
    circuitBreaker.Enabled = true;

    circuitBreaker.FailureRatio = 0.5;

    circuitBreaker.MinimumThroughput = 10;

    circuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

    circuitBreaker.SamplingDuration = TimeSpan.FromMinutes(1);
});
```

## Configuration Options

| Option            | Description                                  | Default      |
| ----------------- | -------------------------------------------- | ------------ |
| Enabled           | Enables or disables the strategy             | `true`       |
| FailureRatio      | Failure threshold before opening the circuit | `0.5`        |
| MinimumThroughput | Minimum requests before evaluation           | `10`         |
| SamplingDuration  | Evaluation window                            | `1 minute`   |
| BreakDuration     | Time the circuit remains open                | `30 seconds` |

---

# Handling Exceptions

Each strategy can be configured to handle only specific exception types.

```csharp
pipeline.AddRetry(retry =>
{
    retry.Handle<TimeoutException>();

    retry.Handle<HttpRequestException>();
});
```

Or multiple exception types.

```csharp
retry.Handle(
    typeof(HttpRequestException),
    typeof(TimeoutException));
```

Only matching exceptions will trigger the configured strategy.

---

# Using appsettings.json

The framework supports configuration binding using the .NET Options pattern.

Example:

```json
{
  "Core": {
    "Resilience": {
      "Pipelines": {
        "Redis": {
          "Retry": {
            "Enabled": true,
            "MaxRetryAttempts": 3,
            "Delay": "00:00:00.500",
            "BackoffType": "Exponential",
            "UseJitter": true
          },
          "Timeout": {
            "Enabled": true,
            "Timeout": "00:00:05"
          },
          "CircuitBreaker": {
            "Enabled": true,
            "FailureRatio": 0.5,
            "MinimumThroughput": 10,
            "SamplingDuration": "00:01:00",
            "BreakDuration": "00:00:30"
          }
        }
      }
    }
  }
}
```

Bind the configuration.

```csharp
builder.Services
    .AddOptions<ResilienceOptions>()
    .Bind(builder.Configuration.GetSection("Core:Resilience"))
    .PostConfigure(options =>
    {
        options.ResolveHandledExceptions();
    });

builder.Services.AddResilience();
```

---

# Resolving Pipelines

Resolve a configured pipeline using dependency injection.

```csharp
public sealed class ProductService(
    IResiliencePipelineProvider provider)
{
    private readonly IResiliencePipeline _pipeline =
        provider.GetPipeline(PipelineType.Redis);
}
```

Execute protected operations.

```csharp
await _pipeline.ExecuteAsync(async cancellationToken =>
{
    await repository.GetAsync(cancellationToken);
});
```

---


## Staging

| Strategy        | Recommendation |
| --------------- | -------------- |
| Retry           | 3 attempts     |
| Timeout         | 10 seconds     |
| Circuit Breaker | Enabled        |

---

## Production

| Strategy        | Recommendation                  |
| --------------- | ------------------------------- |
| Retry           | Exponential backoff with jitter |
| Timeout         | Based on SLA                    |
| Circuit Breaker | Enabled                         |
| Metrics         | Enabled                         |
| OpenTelemetry   | Enabled                         |

---

# Best Practices

✅ Create one pipeline per infrastructure dependency.

✅ Configure retries only for transient failures.

✅ Always combine Retry with Timeout.

✅ Enable Circuit Breaker for external services.

✅ Use exponential backoff with jitter.

✅ Monitor resilience metrics using OpenTelemetry.

✅ Keep pipeline configurations consistent across environments.
