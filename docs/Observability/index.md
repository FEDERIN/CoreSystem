# 📊 CoreSystem.Observability

> **OpenTelemetry and Serilog integration for ASP.NET Core applications on .NET 8**

CoreSystem.Observability configures structured logs, distributed traces,
metrics, and health-check endpoints from a single integration point. Telemetry
is exported through OTLP/gRPC, while other CoreSystem modules can contribute
their own metrics, activity sources, and health checks.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Observability?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-OTLP%20v1.19.0-purple?style=for-the-badge)

---

## 📦 Installation

```bash
dotnet add package CoreSystem.Observability
```

---

## 🚀 Quick Start

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

---

## ✨ Included Capabilities

- Serilog console logging with optional OTLP export
- ASP.NET Core, HTTP client, SQL Server, and runtime instrumentation
- OTLP/gRPC export for traces, metrics, and logs
- Configurable trace sampling and SQL statement capture
- Custom telemetry contributions from CoreSystem modules
- Liveness endpoint at `/health`
- Readiness endpoint at `/ready`

---

## ⚙️ Configuration

Each signal is enabled independently through the `OpenTelemetry` configuration
section:

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
      "Meters": ["Core.Idempotency", "Core.Resilience"]
    }
  }
}
```

An invalid or empty OTLP endpoint skips the exporter without preventing the
application from starting. `CaptureSqlStatements` is disabled by default
because SQL text can contain sensitive data.

---

## 🩺 Health Endpoints

| Endpoint | Purpose |
|---|---|
| `/health` | Liveness probe; verifies that the process can serve requests. |
| `/ready` | Readiness probe; runs health checks contributed by registered modules. |

---

## 🔌 CoreSystem Integration

Register CoreSystem modules before `AddObservability` so their observability
and health-check contributors are included during setup.

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

For the complete package documentation, see the
[CoreSystem.Observability README](https://github.com/FEDERIN/CoreSystem/tree/main/src/Core.Observability).
