# ⚠️ CoreSystem.Http.ProblemDetails

> **Consistent RFC 9457 Problem Details for ASP.NET Core APIs.**

CoreSystem.Http.ProblemDetails provides reusable exception-to-HTTP error
handling for .NET 8 APIs. It owns the HTTP mechanism while each application
keeps its domain exceptions, business error codes, and mapping rules.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Http.ProblemDetails?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Http.ProblemDetails?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

------------------------------------------------------------------------

## ✨ Features

- ✅ RFC 9457 `application/problem+json` responses
- ✅ Application-owned exception mapping through `IExceptionProblemMapper`
- ✅ Standard `errorCode` and `traceId` extensions
- ✅ W3C `Activity.Current.TraceId` with `HttpContext.TraceIdentifier` fallback
- ✅ Application-specific extensions through a callback
- ✅ Exception type and message exposed only in Development
- ✅ No dependency on domain, application, or idempotency packages

------------------------------------------------------------------------

## 📦 Installation

```bash
dotnet add package CoreSystem.Http.ProblemDetails
```

------------------------------------------------------------------------

## 🚀 Quick Start

Implement the mapper in the API or Infrastructure layer. Domain and
Application projects do not need to reference this package.

```csharp
using Core.Http.ProblemDetails;
using Core.Http.ProblemDetails.DependencyInjection;
using Microsoft.AspNetCore.Http;

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
```

Register Problem Details and the mapper-backed exception handler:

```csharp
builder.Services.AddCoreProblemDetails();
builder.Services.AddCoreExceptionHandler<ApiExceptionProblemMapper>();

var app = builder.Build();
app.UseExceptionHandler();
```

`AddCoreExceptionHandler<TMapper>()` registers the concrete mapper as scoped
when it has not already been registered. Register any dependencies required by
the mapper in the usual way. The exception handler creates a scope for each
handled exception, so scoped mapper dependencies (such as a DbContext) are
resolved and disposed correctly.

------------------------------------------------------------------------

## 🔌 Application Extensions

Applications can add fields without introducing domain dependencies into this
package. For example, an API that uses `CoreSystem.Idempotency` can expose the
key related to a fingerprint mismatch:

```csharp
using Core.Idempotency.Exceptions;

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
```

------------------------------------------------------------------------

## 📄 Response Contract

For an exception mapped to a conflict, the handler produces a response similar
to the following:

```json
{
  "type": "urn:example:error:order-already-exists",
  "title": "Order already exists",
  "status": 409,
  "detail": "An order with this identifier already exists",
  "instance": "/orders/42",
  "errorCode": "ORDER_ALREADY_EXISTS",
  "traceId": "4bf92f3577b34da6a3ce929d0e0e4736"
}
```

The handler provides `status`, `title`, `detail`, `type`, `instance`,
`errorCode`, and `traceId`. If the mapper does not specify a type, the handler
uses `about:blank`.

In Development, `exceptionType` and `exceptionMessage` are added. They are
omitted in every other environment.

------------------------------------------------------------------------

## 🏗 Architecture

```text
Domain / Application
        │
        ▼
Application Exception Mapper
        │
        ▼
IExceptionProblemMapper
        │
        ▼
CoreExceptionHandler
        │
        ▼
RFC 9457 Problem Details Response
```

The application owns the vocabulary of errors. CoreSystem owns only reusable
HTTP infrastructure.

------------------------------------------------------------------------

## 📖 Public API

| Type | Description |
|------|-------------|
| `ProblemDescriptor` | Domain-agnostic description of an HTTP problem. |
| `IExceptionProblemMapper` | Maps an exception to a `ProblemDescriptor`. |
| `AddCoreProblemDetails()` | Registers RFC 9457 serialization and customization. |
| `AddCoreExceptionHandler<TMapper>()` | Registers the mapper-backed exception handler. |
| `CoreProblemDetailsOptions` | Provides the `CustomizeProblemDetails` callback. |

------------------------------------------------------------------------

## 📚 Documentation

The full documentation includes:

- Getting Started
- Exception Mapping
- Application Extensions
- Response Contract
- Development Diagnostics
- Best Practices

------------------------------------------------------------------------

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

------------------------------------------------------------------------

## 📄 License

Released under the MIT License.
