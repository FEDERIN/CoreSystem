# FGutierrez.Core.Observability

![NuGet](https://img.shields.io/nuget/v/FGutierrez.Core.Observability?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/FGutierrez.Core.Observability?style=for-the-badge)
![Build](https://img.shields.io/github/actions/workflow/status/your-org/FGutierrez.Core.Observability/ci.yml?style=for-the-badge)
![License](https://img.shields.io/github/license/your-org/FGutierrez.Core.Observability?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-OTLP%20v1.15.1-purple?style=for-the-badge)

---

## 🚀 Overview

FGutierrez.Core.Observability is a high-performance, enterprise-grade .NET observability library designed to standardize telemetry across microservices using OpenTelemetry (OTLP).

It enables metrics, distributed tracing, and structured logging with a single-line integration and full cross-signal correlation.

---

## 🎯 Why This Library?

Using raw OpenTelemetry often results in repetitive boilerplate, inconsistent telemetry, and poor correlation.

This library provides:

- A single unified entry point
- Standardized observability practices
- Automatic Trace ↔ Logs ↔ Metrics correlation

---

## ✨ Features

### OTLP-First Architecture

Built on stable OpenTelemetry Protocol (v1.15.1) with no experimental dependencies.

### Backend-Agnostic Metrics

Push-based OTLP exporter compatible with Grafana, Jaeger, Prometheus, and InfluxDB.

### SQL Deep Insights

Captures SQL queries for performance analysis (configurable to avoid sensitive data).

### Correlated Structured Logging

Serilog integration with TraceId and SpanId enrichment.

### Self-Context Awareness

Auto-detects service name, version, and environment.

### Health Checks

/health and /ready endpoints with structured output.

---

## 📦 Installation

dotnet add package FGutierrez.Core.Observability

---

## 🧑‍💻 Usage

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

## 🏗️ Architecture

App → OTLP → OpenTelemetry Collector → Backend (Grafana, Jaeger, etc.)

---

## 🛠️ Requirements

Requires an OpenTelemetry Collector listening on OTLP gRPC port 4317.

---

## 🤝 Contributing

Fork, create branch, and submit PR.

---

## 📄 License

MIT License

---

## ⭐ Support

Star the repo and share!
