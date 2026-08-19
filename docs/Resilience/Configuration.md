# ⚙️ Configuration

This guide explains how to configure **CoreSystem.Resilience**.

You'll learn how to:

* Register the framework
* Configure resilience pipelines
* Configure Retry, Timeout, and Circuit Breaker strategies
* Register multiple pipelines
* Configure handled exceptions
* Follow recommended production settings

---

# Configuration Overview

The framework is configured through the `AddCoreResilience()` extension.

```csharp
builder.Services.AddCoreResilience(options =>
{
    // Configure your resilience pipelines here.
});
```

A resilience pipeline is identified by a `PipelineType` and can contain one or more resilience strategies.

The available pipeline types provided by the core are:

* `Default`
* `Redis`
* `Sql`
* `Http`
* `Messaging`

---

# Creating a Pipeline

Register a new pipeline by specifying its type and configuring the desired strategies.

```csharp
builder.Services.AddCoreResilience(options =>
{
    options.AddPipeline(PipelineType.Redis, pipeline =>
    {
        pipeline.AddRetry(retry =>
        {
            retry.MaxRetryAttempts = 3;
        });

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

Only strategies that are configured in `PipelineOptions` are added to the pipeline.

The framework builds the configured pipelines when the pipeline registry is initialized.

---

# Registering Multiple Pipelines

Applications can register multiple independent resilience pipelines.

```csharp
builder.Services.AddCoreResilience(options =>
{
    options.AddPipeline(PipelineType.Redis, pipeline =>
    {
        pipeline.Retry = new RetryOptions
        {
            MaxRetryAttempts = 3
        };
    });

    options.AddPipeline(PipelineType.Http, pipeline =>
    {
        pipeline.Timeout = new TimeoutOptions
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
    });
});
```

Each pipeline can be resolved independently through `IResiliencePipelineProvider`.

---

# Retry Configuration

Configure retry behavior for selected exceptions.

```csharp
pipeline.Retry = new RetryOptions
{
    Enabled = true,

    MaxRetryAttempts = 3,

    Delay = TimeSpan.FromMilliseconds(200),

    BackoffType = BackoffType.Exponential,

    UseJitter = false,

    IncludeInnerExceptions = false
};
```

Handled exceptions can be added using `Handle<TException>()` or `Handle(params Type[])`.

```csharp
pipeline.Retry = new RetryOptions
{
    MaxRetryAttempts = 3
}
.Handle<TimeoutException>()
.Handle<HttpRequestException>();
```

## Configuration Options

| Option                 | Description                                        | Default     |
| ---------------------- | -------------------------------------------------- | ----------- |
| Enabled                | Enables or disables the strategy                   | `true`      |
| MaxRetryAttempts       | Maximum retry attempts                             | `3`         |
| Delay                  | Initial retry delay                                | `200 ms`    |
| BackoffType            | Delay calculation strategy                         | Exponential |
| UseJitter              | Enables retry jitter                               | `false`     |
| IncludeInnerExceptions | Includes inner and nested exceptions when matching | `false`     |

Supported backoff types are:

* `Constant`
* `Linear`
* `Exponential`

---

# Timeout Configuration

Configure the maximum execution time allowed for protected operations.

```csharp
pipeline.Timeout = new TimeoutOptions
{
    Timeout = TimeSpan.FromSeconds(5)
};
```

## Configuration Options

| Option  | Description            | Default      |
| ------- | ---------------------- | ------------ |
| Timeout | Maximum execution time | `30 seconds` |

`TimeoutOptions` validates that the configured timeout is greater than zero.

Unlike Retry and Circuit Breaker, Timeout does not have an `Enabled` property. The timeout strategy is configured when `PipelineOptions.Timeout` contains a `TimeoutOptions` instance.

---

# Circuit Breaker Configuration

Protect downstream operations by temporarily blocking requests after repeated failures.

```csharp
pipeline.CircuitBreaker = new CircuitBreakerOptions
{
    Enabled = true,

    FailureRatio = 0.5,

    MinimumThroughput = 10,

    SamplingDuration = TimeSpan.FromSeconds(30),

    BreakDuration = TimeSpan.FromSeconds(15),

    IncludeInnerExceptions = false
};
```

Handled exceptions can be configured in the same way as Retry.

```csharp
pipeline.CircuitBreaker = new CircuitBreakerOptions
{
    FailureRatio = 0.5
}
.Handle<HttpRequestException>()
.Handle<TimeoutException>();
```

## Configuration Options

| Option                 | Description                                        | Default      |
| ---------------------- | -------------------------------------------------- | ------------ |
| Enabled                | Enables or disables the strategy                   | `true`       |
| FailureRatio           | Failure threshold before opening the circuit       | `0.5`        |
| MinimumThroughput      | Minimum executions before evaluation               | `10`         |
| SamplingDuration       | Evaluation window                                  | `30 seconds` |
| BreakDuration          | Time the circuit remains open                      | `15 seconds` |
| IncludeInnerExceptions | Includes inner and nested exceptions when matching | `false`      |

---

# Handling Exceptions

Retry and Circuit Breaker can be configured to handle specific exception types.

```csharp
pipeline.Retry = new RetryOptions
{
    MaxRetryAttempts = 3
}
.Handle<TimeoutException>();
```

Multiple exception types can also be configured.

```csharp
pipeline.Retry = new RetryOptions
{
    MaxRetryAttempts = 3
}
.Handle(
    typeof<HttpRequestException>(),
    typeof(TimeoutException));
```

Only the configured exception types are considered by the strategy.

---

## Matching Inner Exceptions

By default, exception matching checks the exception itself.

When `IncludeInnerExceptions` is enabled, the framework also checks inner and nested exceptions, including exceptions contained in an `AggregateException`.

```csharp
pipeline.Retry = new RetryOptions
{
    IncludeInnerExceptions = true
}
.Handle<TimeoutException>();
```

The same option is available for Circuit Breaker.

```csharp
pipeline.CircuitBreaker = new CircuitBreakerOptions
{
    IncludeInnerExceptions = true
}
.Handle<HttpRequestException>();
```

---

# Disabling Resilience

Resilience can be disabled globally through `ResilienceOptions.Enabled`.

```csharp
builder.Services.AddCoreResilience(options =>
{
    options.Enabled = false;
});
```

When resilience is disabled, the framework registers a `NoOpResiliencePipelineProvider`.

The returned pipeline executes the supplied operation directly without applying Retry, Timeout, or Circuit Breaker strategies.

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

If the requested pipeline has not been registered, `IResiliencePipelineProvider` throws `ResiliencePipelineNotFoundException`.

---

## Staging

| Strategy        | Recommendation                                                      |
| --------------- | ------------------------------------------------------------------- |
| Retry           | Configure according to the dependency                               |
| Timeout         | Based on expected execution time                                    |
| Circuit Breaker | Configure when repeated failures should temporarily block execution |

---

## Production

| Strategy        | Recommendation                                                  |
| --------------- | --------------------------------------------------------------- |
| Retry           | Exponential backoff with appropriate delay                      |
| Timeout         | Based on the expected execution time                            |
| Circuit Breaker | Configure for dependencies where failure protection is required |
| Metrics         | Use the built-in resilience metrics                             |
| OpenTelemetry   | Integrate through the framework's observability registration    |

---

# Best Practices

✅ Configure one pipeline for each infrastructure workload.

✅ Configure retries only for exceptions that should be retried.

✅ Configure a timeout when an operation should have a maximum execution time.

✅ Use Circuit Breaker when repeated failures should temporarily stop execution.

✅ Use exponential backoff when appropriate for retry workloads.

✅ Enable `IncludeInnerExceptions` when wrapped exceptions need to be considered.

✅ Pass the cancellation token supplied to `ExecuteAsync` to the protected operation.
