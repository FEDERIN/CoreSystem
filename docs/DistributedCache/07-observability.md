# 📊 Observability

`CoreSystem.Cache` includes built-in observability based on **OpenTelemetry Metrics**.

Every cache operation automatically emits metrics that can be consumed by any OpenTelemetry-compatible backend, allowing you to monitor cache performance without modifying your application code.

---

# Why Observability Matters

Caching is only effective when it can be measured.

The framework publishes telemetry that helps answer questions such as:

- Is the cache being used effectively?
- What is the cache hit ratio?
- How many cache misses occur?
- Is Redis responding correctly?
- How many cache operations are executed?
- How long do cache operations take?

---

# Architecture

```mermaid
flowchart LR

    Application

    --> Cache

    --> CachePipeline

    --> CacheMetrics

    --> OpenTelemetry

    --> OTLP Exporter

    --> Monitoring Platform
```

---

# Built-in Metrics

The framework publishes the following metrics.

| Metric | Description |
|----------|-------------|
| `cache.distributed.hits` | Successful cache lookups |
| `cache.distributed.misses` | Cache misses |
| `cache.distributed.operations` | Total cache operations |
| `cache.distributed.duration` | Cache execution duration |
| `cache.distributed.hit_rate` | Cache hit ratio |

> Some metrics may be introduced in future releases.

---

# Registering the Meter

The package automatically registers the following meter.

```text
Core.DistributedCache
```

Applications only need to add the meter to OpenTelemetry.

```csharp
builder.Services
    .AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddMeter("Core.DistributedCache");

        metrics.AddPrometheusExporter();
    });
```

---

# Compatible Platforms

The metrics can be exported to any OTLP-compatible backend.

| Platform | Supported |
|-----------|:---------:|
| Prometheus | ✅ |
| Grafana | ✅ |
| Azure Monitor | ✅ |
| Jaeger | ✅ |
| Elastic | ✅ |
| Datadog | ✅ |
| OTLP | ✅ |

---

# Example Dashboard

Typical dashboards include:

- Cache Hits
- Cache Misses
- Hit Ratio
- Average Response Time
- Total Operations

*(Add Grafana screenshots here.)*

---

# Extending Observability

Because every cache operation flows through the **CachePipeline**, new telemetry can be added without modifying the framework.

Examples include:

- Custom business metrics
- Request tracing
- Performance timing
- Audit events
- Telemetry enrichment

Simply create a custom pipeline behavior.

---

# Best Practices

✅ Monitor cache hit ratio.

✅ Track cache misses.

✅ Alert on Redis failures.

✅ Combine metrics with Health Checks.

✅ Export telemetry using OTLP.
