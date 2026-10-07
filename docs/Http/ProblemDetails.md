# CoreSystem.Http.ProblemDetails

> **Consistent RFC 9457 Problem Details for ASP.NET Core APIs.**

CoreSystem.Http.ProblemDetails provides reusable exception-to-HTTP error
handling for .NET 8 APIs. It owns the HTTP mechanism while each application
keeps its domain exceptions, business error codes, and mapping rules.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Http.ProblemDetails?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Http.ProblemDetails?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

---

## Features

- RFC 9457 `application/problem+json` responses
- Application-owned exception mapping through `IExceptionProblemMapper`
- Standard `errorCode` and `traceId` extensions
- W3C `Activity.Current.TraceId` with `HttpContext.TraceIdentifier` fallback
- Application-specific extensions through a callback
- Exception type and message exposed only in Development

---

## Installation

```bash
dotnet add package CoreSystem.Http.ProblemDetails
```

---

## Quick Start

Implement the mapper in the API or Infrastructure layer. Domain and
Application projects do not need to reference this package.

```csharp
public sealed class ApiExceptionProblemMapper : IExceptionProblemMapper
{
    public ProblemDescriptor Map(Exception exception) => exception switch
    {
        OrderNotFoundException => new(404, "order_not_found", "Order not found", $"The order was not found."),
        _ => new(500, "internal_error", "Internal error", "An unexpected error occurred"),
    };
}
```

Register the handler and mapper:

```csharp
builder.Services.AddCoreProblemDetails(new ApiExceptionProblemMapper());

app.UseCoreExceptionHandler();
```

> When the exception mapper is registered via DI, an alternative for the
> `UseCoreExceptionHandler` flow is `AddCoreExceptionHandler<TMapper>()`, which
> returns the handler from the same registration entry point.

> Refer to the full package README for the complete Quick Start example,
> including custom problem extensions and options.
