# ⚡ CoreSystem.Http

> **Reusable capture and replay of HTTP responses for ASP.NET Core.**

CoreSystem.Http captures and replays HTTP responses in ASP.NET Core applications
without repeating response-interception boilerplate. It has no dependencies
beyond ASP.NET Core, so features such as caching, idempotency or auditing can
build on it directly.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Http?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Http?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-blue?style=for-the-badge)

---

## ✨ Features

- ✅ Capture responses from the ASP.NET Core pipeline
- ✅ Replay captured responses
- ✅ Response body buffering
- ✅ Header preservation
- ✅ HEAD request support
- ✅ Lightweight and dependency-free
- ✅ Built for ASP.NET Core
- ✅ Reusable across middleware, libraries and application frameworks

---

## 📦 Installation

```bash
dotnet add package CoreSystem.Http
```

---

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

---

## 📖 Public API

CoreSystem.Http intentionally exposes a small, focused and stable public API.

| Type | Kind | Description |
|------|------|-------------|
| `IResponseCapture` | Interface | Captures the HTTP response produced by the ASP.NET Core request pipeline. |
| `IHttpResponseWriter` | Interface | Replays a previously captured response to the current `HttpContext`. |
| `CapturedResponse` | Model | Represents a captured HTTP response, including the status code, headers, content type, and response body. |
| `AddCoreHttp()` | Extension Method | Registers all CoreSystem.Http services required for response capture and replay. |

---

## 🧾 Captured Headers

Capture copies every response header except hop-by-hop and transport headers:
`Transfer-Encoding`, `Content-Length`, `Connection`, `Date`, `Server`,
`Keep-Alive`, `Upgrade`, `TE`, `Trailer`, `Proxy-Authenticate` and
`Proxy-Authorization`. Replay sets `Content-Length` from the captured body. For a
HEAD request, replay writes the status and headers but no body.

---

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
      ▼
IHttpResponseWriter
      │
      ▼
HTTP Response
```

---

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

---

## 🛠 Requirements

CoreSystem.Http has no external runtime dependencies beyond ASP.NET Core.

---

## 📚 Documentation

- [Overview](https://federin.github.io/CoreSystem/Http/)
- [Getting Started](https://federin.github.io/CoreSystem/Http/GettingStarted/)
- [Architecture](https://federin.github.io/CoreSystem/Http/Architecture/)
- [Problem Details](https://federin.github.io/CoreSystem/Http/ProblemDetails/)

---

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

---

## 📄 License

Released under the MIT License.
