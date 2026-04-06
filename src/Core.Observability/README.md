# Core.Observability

A high-performance, reusable **.NET Observability Library** designed to standardize telemetry across microservices. It provides a "single-line" integration for **Distributed Tracing**, **Metrics**, and **Structured Logging**.

---

## 🚀 Key Features

* **Distributed Tracing**: Automatic instrumentation for ASP.NET Core and HttpClient, exporting to **Jaeger** via OTLP.
* **Metrics Engine**: Real-time performance monitoring (CPU, RAM, HTTP) ready for **Prometheus** scraping.
* **Structured Logging**: Enhanced **Serilog** configuration that automatically correlates Logs with `TraceId` and `SpanId`.
* **Auto-Context**: Dynamically detects Service Name and Environment from the entry assembly.

---

## 📦 Installation

To package the library for local or private NuGet distribution:

```bash
dotnet pack -c Release -o ./dist
