# 🌐 CoreSystem.Http

> **Build HTTP features once. Reuse them everywhere.**

Production-ready HTTP infrastructure for ASP.NET Core and .NET 8.

CoreSystem.Http provides reusable infrastructure for response capture, replay,
and RFC 9457 Problem Details in ASP.NET Core applications. It offers focused
components that simplify response interception and consistent exception handling
while promoting code reuse across middleware, pipelines, and application frameworks.

Designed as a foundational building block, CoreSystem.Http can be used by
caching, idempotency, auditing, logging, security, or any feature that requires
capturing HTTP responses, reproducing them, or translating application exceptions
to HTTP errors without coupling CoreSystem to domain code.

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
- ✅ RFC 9457 (`application/problem+json`) exception handling
- ✅ Application-owned exception mapping, error codes, and extensions
- ✅ Trace identifiers and safe Development-only exception diagnostics

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

### Problem Details

Define the HTTP representation of your application's exceptions. Domain-specific
exceptions and mappers remain in the application; CoreSystem only owns the HTTP
mechanism.

```csharp
using Core.Http.ProblemDetails;

public sealed class ApiExceptionProblemMapper : IExceptionProblemMapper
{
    public ProblemDescriptor Map(Exception exception) => exception switch
    {
        OrderNotFoundException => new(
            StatusCodes.Status404NotFound,
            "ORDER_NOT_FOUND",
            "Order not found",
            "The requested order does not exist",
            "urn:example:error:order-not-found"),
        _ => new(
            StatusCodes.Status500InternalServerError,
            "INTERNAL_ERROR",
            "Unexpected error",
            "An unexpected error occurred")
    };
}

builder.Services.AddCoreProblemDetails(options =>
{
    options.CustomizeProblemDetails = (problem, context, exception) =>
    {
        if (exception is IdempotencyFingerprintMismatchException)
        {
            problem.Extensions["idempotencyKey"] =
                context.Request.Headers["Idempotency-Key"].ToString();
        }
    };
});
builder.Services.AddCoreExceptionHandler<ApiExceptionProblemMapper>();

var app = builder.Build();
app.UseExceptionHandler();
```

The `IdempotencyFingerprintMismatchException` branch is optional and belongs in
an application that references `CoreSystem.Idempotency`; CoreSystem.Http itself
does not take a dependency on idempotency or any domain package.

The handler returns `application/problem+json` with `status`, `title`, `detail`,
`type`, `instance`, `errorCode`, and `traceId`. It adds `exceptionType` and
`exceptionMessage` only in the Development environment.

------------------------------------------------------------------------

## 📖 Public API

CoreSystem.Http intentionally exposes a small, focused and stable public API.

| Type | Kind | Description |
|------|------|-------------|
| `IResponseCapture` | Interface | Captures the HTTP response produced by the ASP.NET Core request pipeline. |
| `IHttpResponseWriter` | Interface | Replays a previously captured response to the current `HttpContext`. |
| `CapturedResponse` | Model | Represents a captured HTTP response, including the status code, headers, content type, and response body. |
| `AddCoreHttp()` | Extension Method | Registers all CoreSystem.Http services required for response capture and replay. |
| `ProblemDescriptor` | Model | Domain-agnostic description of an HTTP problem. |
| `IExceptionProblemMapper` | Interface | Maps application exceptions to `ProblemDescriptor` instances. |
| `AddCoreProblemDetails()` | Extension Method | Registers RFC 9457 serialization and optional extension customization. |
| `AddCoreExceptionHandler<TMapper>()` | Extension Method | Registers the mapper-backed ASP.NET Core exception handler. |

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
- Consistent API error contracts across services

------------------------------------------------------------------------

## Dependencies

CoreSystem.Http depends on ASP.NET Core and Microsoft.Extensions.Options.
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

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

------------------------------------------------------------------------

## 📄 License

Released under the MIT License.
