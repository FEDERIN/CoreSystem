# 📊 CoreSystem.Observability

> **OpenTelemetry and Serilog integration for ASP.NET Core applications on .NET 8**

CoreSystem.Observability configures structured logs, distributed traces, metrics,
and health-check endpoints from a single `WebApplicationBuilder` extension. It
exports telemetry through OTLP/gRPC and supports observability contributions
registered by other CoreSystem modules.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Observability?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Observability?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-OTLP%20v1.19.0-purple?style=for-the-badge)

---

## ✨ Features

- ✅ Serilog console logging with optional OTLP log export
- ✅ ASP.NET Core, HTTP client, SQL Server, and runtime instrumentation
- ✅ OTLP/gRPC export for traces, metrics, and logs
- ✅ Configurable trace sampling
- ✅ Optional SQL statement capture, disabled by default
- ✅ Custom meters and activity sources contributed by CoreSystem modules
- ✅ Liveness endpoint at `/health`
- ✅ Readiness endpoint at `/ready`
- ✅ Invalid or empty OTLP endpoints do not prevent the application from starting

---

## 📦 Installation

```bash
dotnet add package CoreSystem.Observability
```

---

## 🚀 Quick Start

Register observability before building the application:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddObservability(
    environment: builder.Environment.EnvironmentName,
    serviceName: "Orders.Api",
    serviceNamespace: "Company.Platform");

var app = builder.Build();

app.UseObservabilityEndpoints();

app.Run();
```

`UseObservabilityEndpoints()` enables Serilog request logging and maps the
health endpoints. Place it before endpoint mappings that should be included in
request logging.

---

## ⚙️ Configuration

Configure telemetry in the `OpenTelemetry` section of `appsettings.json`.
Each signal is disabled unless its `Enabled` property is `true`.

```json
{
  "OpenTelemetry": {
    "Logging": {
      "Enabled": true,
      "OtlpEndpoint": "http://localhost:4317"
    },
    "Tracing": {
      "Enabled": true,
      "OtlpEndpoint": "http://localhost:4317",
      "SamplingProbability": 1.0,
      "CaptureSqlStatements": false
    },
    "Metrics": {
      "Enabled": true,
      "OtlpEndpoint": "http://localhost:4317",
      "Meters": [
        "Core.Idempotency",
        "Core.Resilience"
      ]
    }
  }
}
```

All three signals use OTLP over gRPC. The default endpoint is
`http://otel-collector:4317`; provide an endpoint reachable from the running
application. An empty or invalid endpoint skips that signal's exporter while
leaving the application running.

### Trace sampling

`SamplingProbability` must be between `0` and `1`. Invalid values fall back to
`1.0`.

### SQL statement capture

`CaptureSqlStatements` is `false` by default. SQL text can contain sensitive
values, so enable it only after confirming your queries do not expose secrets
or personally identifiable information.

---

## 🩺 Health Endpoints

| Endpoint | Purpose | Registered checks |
|---|---|---|
| `/health` | Liveness probe | None; reports that the process can serve requests. |
| `/ready` | Readiness probe | All checks registered by `IHealthCheckContributor` implementations. |

Both endpoints return JSON with the overall status, duration, and individual
check details. The health and readiness endpoints are excluded from ASP.NET
Core trace instrumentation to reduce telemetry noise.

---

## 📊 Telemetry

### Traces

Tracing includes ASP.NET Core requests, outgoing `HttpClient` calls, and SQL
Server client operations. Registered `IObservabilityContributor` implementations
can add their activity source names to the trace provider.

### Metrics

Metrics include ASP.NET Core, `HttpClient`, and .NET runtime instrumentation.
Add application or CoreSystem meter names through `OpenTelemetry:Metrics:Meters`.

### Logs

Logs use Serilog with console output by default. When logging is enabled and a
valid OTLP endpoint is configured, logs are additionally exported through OTLP.

---

## 🔌 CoreSystem Integration

CoreSystem modules can contribute their own telemetry and health checks through
the `CoreSystem.Observability.Abstractions` package. Register those modules
before calling `AddObservability` so their contributors are included during
observability setup.

```csharp
builder.Services.AddCoreIdempotency(options =>
{
    // Configure the module.
});

builder.AddObservability(
    builder.Environment.EnvironmentName,
    "Orders.Api",
    "Company.Platform");
```

---

## 🛠 Requirements

| Component | Version |
|---|---|
| .NET SDK | 8.0+ |
| ASP.NET Core | 8.0+ |
| OTLP collector or compatible backend | Recommended |
| OTLP gRPC endpoint | Commonly port 4317 |

---

## 🤝 Contributing

Issues, discussions, and pull requests are welcome.

---

## 📄 License

Released under the MIT License.
