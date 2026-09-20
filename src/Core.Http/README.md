# 🌐 CoreSystem.Http

> **Build HTTP features once. Reuse them everywhere.**

Production-ready HTTP infrastructure for ASP.NET Core and .NET 8.

CoreSystem.Http provides reusable infrastructure for capturing and replaying
HTTP responses in ASP.NET Core applications. It offers lightweight,
dependency-free components that simplify response interception while promoting
code reuse across middleware, pipelines, and application frameworks.

Designed as a foundational building block, CoreSystem.Http can be used by
caching, idempotency, auditing, logging, security, or any feature that requires
capturing or reproducing HTTP responses.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Http?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Http?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

------------------------------------------------------------------------

## Why use CoreSystem.Http?

ASP.NET Core provides powerful HTTP primitives, but reusable response capture and replay often require repetitive boilerplate code.


CoreSystem.Http extracts these capabilities into reusable building blocks that can be shared across multiple libraries and applications.
------------------------------------------------------------------------

## ✨ Features

- ✅ Capture responses from the ASP.NET Core pipeline
- ✅ Replay captured responses
- ✅ Response body buffering
- ✅ Header preservation
- ✅ HEAD request support
- ✅ Lightweight and dependency-free
- ✅ Built for ASP.NET Core
- ✅ Reusable across middleware, libraries and application frameworks

------------------------------------------------------------------------

## 📦 Installation

```bash
dotnet add package CoreSystem.Http
```

------------------------------------------------------------------------

## 🚀 Quick Start

Register CoreSystem.Http:

```csharp
builder.Services.AddCoreHttp();
```

Inject the required services:

```csharp
public sealed class MyHandler(
    IResponseCapture responseCapture,
    IHttpResponseWriter responseWriter)
{
}
```

Capture an HTTP response:

```csharp
CapturedResponse response =
    await responseCapture.CaptureAsync(
        context,
        next,
        cancellationToken);
```

Replay the captured response:

```csharp
await responseWriter.WriteAsync(
    context,
    response,
    cancellationToken);
```

------------------------------------------------------------------------

## 📖 Public API

CoreSystem.Http intentionally exposes a small, focused and stable public API.

| Type | Kind | Description |
|------|------|-------------|
| `IResponseCapture` | Interface | Captures the HTTP response produced by the ASP.NET Core request pipeline. |
| `IHttpResponseWriter` | Interface | Replays a previously captured response to the current `HttpContext`. |
| `CapturedResponse` | Model | Represents a captured HTTP response, including the status code, headers, content type, and response body. |
| `AddCoreHttp()` | Extension Method | Registers all CoreSystem.Http services required for response capture and replay. |

------------------------------------------------------------------------

## 🏗 Architecture

CoreSystem.Http separates response capture from response replay, allowing both components to be reused independently.

```text
HTTP Request
      │
      ▼
ASP.NET Core Pipeline
      │
      ▼
IResponseCapture
      │
      ▼
CapturedResponse
      │
      ├───────────────┐
      │               │
      ▼               ▼
CoreSystem.Cache      CoreSystem.Idempotency
      │               │
      └───────────────┘
              │
              ▼
      IHttpResponseWriter
              │
              ▼
        HTTP Response
```

------------------------------------------------------------------------

## 🎯 Use Cases

CoreSystem.Http serves as reusable infrastructure for features such as:

- HTTP response caching
- Idempotency
- Audit logging
- Response transformation
- API gateways
- Reverse proxies
- Middleware development
- Custom ASP.NET Core frameworks

------------------------------------------------------------------------

## Dependencies

CoreSystem.Http has no external runtime dependencies beyond ASP.NET Core.
------------------------------------------------------------------------

## 📚 Documentation

The full documentation includes:

- Getting Started
- Architecture
- Dependency Injection
- Response Capture
- Response Replay
- Extensibility
- Best Practices

## 📚 Documentation

Documentation is continuously expanding as CoreSystem evolves.

Additional guides, architecture notes and examples will be available in the project's documentation site.

------------------------------------------------------------------------

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

------------------------------------------------------------------------

## 📄 License

Released under the MIT License.