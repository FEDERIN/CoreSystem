# ⚡ CoreSystem.Idempotency

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Idempotency?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Idempotency?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-Enabled-purple?style=for-the-badge)
![Storage](https://img.shields.io/badge/Storage-Redis%20%7C%20PostgreSQL-green?style=for-the-badge)

---

# 📖 Overview

**CoreSystem.Idempotency** is a production-ready idempotency framework for ASP.NET Core that guarantees critical operations are executed **exactly once**, even when clients retry requests because of timeouts, network failures, or duplicate submissions.

The framework transparently intercepts incoming requests, validates request fingerprints, persists successful responses, and safely replays previously completed operations.

Designed for distributed systems, it provides provider-independent storage, built-in OpenTelemetry instrumentation, and a highly extensible architecture.

---

# ✨ Features

- ✅ ASP.NET Core middleware
- ✅ Exactly-once request processing
- ✅ Response replay
- ✅ Request fingerprint validation
- ✅ Duplicate request detection
- ✅ Redis provider
- ✅ PostgreSQL provider
- ✅ Configurable expiration
- ✅ Configurable HTTP methods
- ✅ Configurable request fingerprinting
- ✅ OpenTelemetry Metrics
- ✅ Production-ready architecture

---

# 🚀 Quick Start

Install the package

```bash
dotnet add package CoreSystem.Idempotency
```

Register the services

```csharp
builder.Services.AddIdempotencyProvider(builder.Configuration);
```

Enable the middleware

```csharp
app.UseIdempotency();
```

Configure the provider

```json
{
  "Idempotency": {
    "Enabled": true,
    "Provider": "Redis",
    "HeaderName": "X-Idempotency-Key",
    "AllowedMethods": [ "POST", "PUT", "DELETE" ],
    "Expiration": "06:00:00"
  }
}
```

That's all.

---

# 🔒 How It Works

1. The client sends a request with an **Idempotency-Key**.
2. The middleware checks the configured storage provider.
3. If the key already exists:
   - The request fingerprint is validated.
   - The stored response is replayed.
4. Otherwise:
   - The request executes normally.
   - The response is persisted.
   - Future retries return the stored response.

---

# 📦 Supported Providers

| Provider | Status |
|----------|--------|
| Redis | ✅ |
| PostgreSQL | ✅ |

---

# 📊 Observability

CoreSystem.Idempotency publishes OpenTelemetry metrics out of the box.

Available telemetry includes:

- Request processing
- Duplicate detection
- Response replay
- Storage latency
- Payload size

Compatible with:

- OpenTelemetry
- Prometheus
- Grafana
- Jaeger

---

# 📚 Documentation

Complete documentation is available in the project documentation.

It includes:

- Getting Started
- Architecture
- Configuration
- Fingerprinting
- Response Replay
- Storage Providers
- Observability
- Best Practices
- Error Reference
- Roadmap

---

# 🤝 Contributing

Contributions, bug reports, and feature requests are welcome.

If you'd like to contribute:

1. Fork the repository.
2. Create a feature branch.
3. Submit a Pull Request.

---

# 📄 License

MIT License © Federin Pastor Gutierrez Ortiz