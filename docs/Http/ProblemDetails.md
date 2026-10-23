# CoreSystem.Http.ProblemDetails

> **Consistent RFC 9457 Problem Details for ASP.NET Core APIs.**

CoreSystem.Http.ProblemDetails provides reusable exception-to-HTTP error
handling for ASP.NET Core APIs on .NET 8, .NET 9 and .NET 10. It owns the HTTP mechanism while each application
keeps its domain exceptions, business error codes, and mapping rules.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Http.ProblemDetails?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Http.ProblemDetails?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-blue?style=for-the-badge)

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

> Refer to the full package README for the complete Quick Start example,
> including custom problem extensions and options.
