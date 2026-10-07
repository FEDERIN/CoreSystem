# ⚡ CoreSystem.Correlation

> **Correlation ID propagation middleware for ASP.NET Core.**

CoreSystem.Correlation is a lightweight, standalone library that accepts or issues a correlation identifier per request, exposes it via `HttpContext.Items`, and enriches log scopes with it. It can also be composed with [CoreSystem.Observability](https://federin.github.io/CoreSystem/Observability).

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Correlation?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Correlation?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

---

## ✨ Features

- ✅ Accepts a client-provided correlation header (configurable name)
- ✅ Generates a reliable `Guid`-based ID otherwise
- ✅ Exposes the ID via `HttpContext.Items[CoreCorrelationConstants.HttpContextItemKey]`
- ✅ Echoes the ID back in the response headers
- ✅ Enriches log scopes with a `correlationId` property
- ✅ Input validation (length + character allowlist)
- ✅ `IOptions`/`IConfiguration` binding
- ✅ Standalone — no dependency on CoreSystem.Observability

---

## 📦 Installation

```bash
dotnet add package CoreSystem.Correlation
```

---

## 🚀 Quick Start

Register the services:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreCorrelation(); // or builder.Services.AddCoreCorrelation(o => o.MaximumLength = 64);

var app = builder.Build();
```

Add the middleware:

```csharp
app.UseCoreCorrelationId(); // after UseRouting/UseCors, before endpoints
```

Read the current ID anywhere in the pipeline:

```csharp
app.MapGet("/ping", (HttpContext ctx) =>
    Results.Ok(ctx.Items[CoreCorrelationConstants.HttpContextItemKey]));
```

---

## ⚙ Configuration

Bind from the `Core:Correlation` configuration section:

```csharp
builder.Services.AddCoreCorrelation(builder.Configuration);
```

| Property | Default | Description |
|----------|---------|-------------|
| `Enabled` | `true` | Enables correlation propagation |
| `HeaderName` | `X-Correlation-Id` | Header read from (and written back to) the request |
| `IncludeInResponse` | `true` | Whether the ID is echoed in the response |
| `MaximumLength` | `128` | Maximum accepted length for a client-provided ID |

---

## 🏗 Architecture

```text
Client Request
     │
     ▼
CorrelationIdMiddleware
     │
     ├── validates / generates id
     ├── context.Items["CoreSystem.CorrelationId"]
     ├── logger scope (correlationId)
     ▼
Request pipeline
```

Each request gets one ID for its whole pipeline. The value flows through `HttpContext.Items` and log scopes so application code can attach it to logs and downstream calls without HTTP-specific knowledge.

---

## 📖 Documentation

The full documentation includes:

- Getting Started
- Configuration
- Log enrichment
- Composing with CoreSystem.Observability
- Architecture

Visit the GitHub repository for the complete documentation.

---

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

---

## 📄 License

Released under the MIT License.
