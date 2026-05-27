# 📊 FGutierrez.Core.Observability

![NuGet](https://img.shields.io/nuget/v/FGutierrez.Core.Observability?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/FGutierrez.Core.Observability?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-OTLP%20v1.15.1-purple?style=for-the-badge)

---

# 🚀 Overview

**FGutierrez.Core.Observability** is a high-performance .NET observability library designed to standardize telemetry across distributed systems using OpenTelemetry.

The package provides a unified and production-ready integration layer for:

- Distributed tracing
- Metrics collection
- Structured logging
- Health monitoring
- Cross-signal correlation

with minimal setup and cloud-native compatibility.

---

# 🎯 Why This Library?

Configuring OpenTelemetry manually across multiple services often introduces:

- repetitive boilerplate
- inconsistent telemetry standards
- fragmented diagnostics
- missing trace correlation

This library solves those issues by providing a centralized observability foundation for .NET microservices.

---

# ✨ Features

## 🔭 OpenTelemetry Native

Built on stable OpenTelemetry SDKs with OTLP-first architecture.

---

## 📈 Metrics Collection

Automatic instrumentation for:

- ASP.NET Core
- Runtime metrics
- HTTP clients
- Custom application meters

---

## 🧾 Distributed Tracing

Captures and exports traces with:

- TraceId propagation
- Span correlation
- OTLP exporter support

---

## 🪵 Structured Logging

Integrated Serilog pipeline with:

- TraceId enrichment
- SpanId enrichment
- JSON structured logs

---

## 🩺 Health Monitoring

Built-in endpoints:

```text
/health
/ready
```

with standardized responses.

---

## 🧠 Self-Context Awareness

Automatically detects:

- Service name
- Environment
- Service version

---

## 🗄️ SQL Observability

Captures SQL activity for performance analysis.

Configurable to avoid sensitive data exposure.

---

## 🔗 Ecosystem Integration

Native integration with:

```text
FGutierrez.Core.Idempotency
```

Metrics and traces are automatically correlated without additional configuration.

---

## 🛡️ Resilience-First Startup

The library validates configuration and OTLP connectivity safely during startup.

If exporters or collectors are unavailable:

- the application continues running
- telemetry degrades gracefully
- no startup crashes occur

---

# 📦 Installation

```bash
dotnet add package FGutierrez.Core.Observability
```

---

# ⚙️ Configuration

## appsettings.json

```json
{
  "OpenTelemetry": {
    "Tracing": {
      "Enabled": true,
      "OtlpEndpoint": "http://localhost:4317"
    },
    "Metrics": {
      "Enabled": true,
      "OtlpEndpoint": "http://localhost:4317",
      "Meters": [
        "FGutierrez.Core.Idempotency"
      ]
    },
    "Logging": {
      "Enabled": true,
      "OtlpEndpoint": "http://localhost:4317"
    }
  }
}
```

---

# 🧑‍💻 Quick Start

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddObservability(
    environment: builder.Environment.EnvironmentName,
    serviceName: "OrderService",
    serviceNamespace: "Company.Core"
);

var app = builder.Build();

app.UseObservabilityEndpoints();

app.Run();
```

---

# 🏗️ Included Components

```text
Extensions/
├── HealthCheckEndpointsExtensions.cs
├── HealthCheckExtensions.cs
├── OpenTelemetryMetricsExtensions.cs
├── OpenTelemetryTracingExtensions.cs
└── SerilogExtensions.cs
```

---

## 🏗️ Architecture

![Observability Flow](https://raw.githubusercontent.com/FEDERIN/CoreSystem/main/docs/observability-flow.png)

---

# 📊 Supported Signals

| Signal | Support |
|---|---|
| Traces | ✅ |
| Metrics | ✅ |
| Logs | ✅ |
| Health Checks | ✅ |
| Correlation IDs | ✅ |

---

# 🛠️ Requirements

| Component | Version |
|---|---|
| .NET SDK | 8.0+ |
| OpenTelemetry Collector | Recommended |
| OTLP gRPC Endpoint | Port 4317 |

---

# 🚀 Recommended Stack

This library works especially well with:

- OpenTelemetry Collector
- Grafana
- Prometheus
- Jaeger
- Loki
- Docker Compose
- Kubernetes

---

# 🧪 Engineering Principles

FGutierrez.Core.Observability follows:

- Observability-First architecture
- Cloud-native engineering
- Middleware-first integrations
- Production-grade defaults
- Minimal boilerplate
- Vendor-neutral telemetry

---

# 🤝 Contributing

Contributions and improvements are welcome.

## Development Flow

1. Fork the repository
2. Create a feature branch
3. Commit changes
4. Submit a Pull Request

---

# 📄 License

MIT License © Federin Pastor Gutierrez Ortiz

---

# ⭐ Support

If this project helps you, consider giving it a star on GitHub.	