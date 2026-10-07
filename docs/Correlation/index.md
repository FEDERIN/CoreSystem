# CoreSystem.Correlation

> **Correlation-id propagation middleware for ASP.NET Core.**

CoreSystem.Correlation is a lightweight, standalone middleware that accepts or issues a correlation identifier per request, exposes it via `HttpContext.Items`, and enriches log scopes with it. It can be used on its own, or alongside [CoreSystem.Observability](../Observability/index.md).

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Correlation?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Correlation?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

---

## Installation

```bash
dotnet add package CoreSystem.Correlation
```

---

## Quick Start

```csharp
builder.Services.AddCoreCorrelation();

app.UseCoreCorrelationId();

app.MapGet("/ping", (HttpContext ctx) =>
    Results.Ok(ctx.Items[CoreCorrelationConstants.HttpContextItemKey]));
```

---

## Configuration

```csharp
builder.Services.AddCoreCorrelation(builder.Configuration);
```

| Property | Default | Description |
|---|---|---|
| `Enabled` | `true` | Enables correlation propagation |
| `HeaderName` | `X-Correlation-Id` | Header read from (and written back to) the request |
| `IncludeInResponse` | `true` | Whether the ID is echoed in the response |
| `MaximumLength` | `128` | Maximum accepted length for a client-provided ID |

---

## Capabilities

- Honors a client-provided correlation header (configurable name)
- Generates a new ID otherwise
- Echoes the ID back in the response headers
- Enrich log scopes with a `correlationId` property
- Input validation (length + character allowlist)
