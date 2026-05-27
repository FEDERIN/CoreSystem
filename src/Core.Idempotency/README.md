# ⚡ FGutierrez.Core.Idempotency

![NuGet](https://img.shields.io/nuget/v/FGutierrez.Core.Idempotency?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/FGutierrez.Core.Idempotency?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-OTLP%20v1.15.1-purple?style=for-the-badge)
![Storage](https://img.shields.io/badge/Storage-Redis%20%7C%20PostgreSQL-green?style=for-the-badge)


---

# 📖 Overview

**FGutierrez.Core.Idempotency** is a high-performance .NET 8 library designed to guarantee that critical operations such as payments, order creation, and transactional workflows are executed **exactly once**, even in scenarios involving retries, duplicated requests, or network failures.

The library provides a distributed idempotency engine with multi-provider persistence support and built-in observability integrations.

---

# 🎯 Why This Library?

Implementing idempotency manually often introduces:

- Boilerplate middleware
- Duplicate business execution
- Inconsistent storage logic
- Poor traceability
- Lack of metrics and diagnostics

This library solves those challenges through a unified and extensible architecture.

---

# ✨ Features

| Feature | Description |
|---|---|
| ⚡ High Performance | Optimized request interception pipeline |
| 🗄 Multi-Provider | Redis or PostgreSQL support |
| 📊 Observability | OpenTelemetry metrics & tracing |
| ♻ Response Caching | Stores and replays original responses |
| 🔒 Duplicate Prevention | Prevents re-execution of business operations |
| 🧩 Extensible | Provider-based architecture |
| 🏗 Auto Schema | Automatic PostgreSQL schema generation |

---

# 🧠 Architecture

## 🏗️ Architecture & Workflow

![Idempotency Flow](https://raw.githubusercontent.com/FEDERIN/CoreSystem/main/docs/idempotency-flow.png)

---

# 📦 Supported Providers

| Provider | Technology |
|---|---|
| PostgreSQL | Dapper |
| Redis | StackExchange.Redis |

---

# 📊 Observability

The library exposes native OpenTelemetry instrumentation for:

- Request duration
- Cache hits/misses
- Duplicate detection
- Storage latency
- Exception tracking

Compatible with:

- Jaeger
- Prometheus
- Grafana
- OTLP Collectors

---

# 📦 Installation

```bash
dotnet add package FGutierrez.Core.Idempotency
```

---

# ⚙️ Configuration

## 🛠️ Infrastructure Setup (PostgreSQL)

> [!IMPORTANT]
> **Database Provisioning:** The library automatically creates the required table (`idempotency_keys`) and indexes, but it **requires the physical database to exist** in the server.

Before running the application, ensure the database specified in your connection string is created:

```sql
CREATE DATABASE idempotency_db; -- or the name in your connection string (ConnectionStrings.Idempotency)
```

### 🕒 Expiration Format (`TimeSpan`)

The `Expiration` field follows the standard .NET `TimeSpan` string format: `[d.]hh:mm:ss`. 

> [!CAUTION]
> **Hour Overflow:** In .NET, the hour component must be between 0 and 23. If you specify `24:00:00`, the system will interpret it as **24 days** instead of 24 hours.

To avoid misconfigurations, use the following guide:

| Desired Duration | Config Value | Explanation |
| :--- | :--- | :--- |
| **1 Hour** | `01:00:00` | Standard `hh:mm:ss` |
| **23 Hours** | `23:00:00` | Maximum hour value |
| **24 Hours (1 Day)** | `1.00:00:00` | Use `d.hh:mm:ss` (note the dot) |
| **30 Hours** | `1.06:00:00` | 1 Day + 6 Hours |
| **7 Days** | `7.00:00:00` | 7 Days |

## appsettings.json

```json
{
  "ConnectionStrings": {
    "Idempotency": "Host=localhost;Port=5432;Database=idempotency_db;Username=admin;Password=admin"
  },
  "Idempotency": {
    "Enabled": true,
    "Provider": "PostgreSQL",
    "AllowedMethods": [ "POST", "PUT", "DELETE" ],
    "HeaderName": "X-Idempotency-Key",
    "Expiration": "06:00:00",
    "MeterName": "FGutierrez.Core.Idempotency"
  }
}
```

---
# 🚀 Quick Start

## Register Services

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdempotencyProvider(builder.Configuration);

var app = builder.Build();
```

---

## Enable Middleware

```csharp
app.UseIdempotency();

app.Run();
```

---

# ⚙️ Request Lifecycle

1. Client sends a request with `X-Idempotency-Key`
2. Middleware checks storage provider
3. If the key exists:
   - Cached response is returned
4. If the key does not exist:
   - Business logic executes normally
   - Response is persisted
   - Response is returned to the client
5. Telemetry is exported automatically

---

# 🛠 Requirements

- .NET 8 SDK
- PostgreSQL 16+ or Redis 7+
- Optional OpenTelemetry Collector

---

# 🧪 Future Roadmap

- [ ] SQL Server Provider
- [ ] MongoDB Provider
---

# 🤝 Contributing

Contributions are welcome.

To contribute:

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Open a Pull Request

---

# 📄 License

MIT License © Federin Pastor Gutierrez Ortiz

---

# ⭐ Support

If this project helped you, consider giving it a star on GitHub.