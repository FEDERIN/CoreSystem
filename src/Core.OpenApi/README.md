# 🧭 CoreSystem.OpenApi

> **Generate the OpenAPI document once. Reuse the conventions everywhere.**

CoreSystem.OpenApi provides the OpenAPI and Swagger infrastructure shared by
CoreSystem-based APIs on .NET 8, .NET 9 and .NET 10. It owns the document wiring,
the response attributes and the filters, while each application keeps its own
title, servers, examples and error vocabulary.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.OpenApi?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.OpenApi?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-blue?style=for-the-badge)

------------------------------------------------------------------------

## ✨ Features

- ✅ One-call Swagger generation wiring (`AddCoreOpenApi`) and hosting (`UseCoreOpenApi`)
- ✅ Bearer security scheme applied to every operation
- ✅ `ProducesOk<T>`, `ProducesCreated<T>` and `ProducesNoContent` response attributes
- ✅ `Location` header documented on every `201 Created` response
- ✅ Redundant `text/plain` and `text/json` representations removed
- ✅ Shared problem-details responses declared once as `$ref` components
- ✅ Configurable error codes, type URIs, examples and schemas
- ✅ Patched document cached in memory instead of rebuilt per request
- ✅ Invalid JSON is returned unchanged, so the patcher is safe to compose

------------------------------------------------------------------------

## 📦 Installation

```bash
dotnet add package CoreSystem.OpenApi
```

------------------------------------------------------------------------

## 🚀 Quick Start

Register the document and map the endpoint:

```csharp
using Core.OpenApi.DependencyInjection;
using Core.OpenApi.Extensions;
using Core.OpenApi.Options;

builder.Services.AddCoreOpenApi(options =>
{
    options.Title = "Orders API";
    options.Description = "Order management endpoints";
    options.Servers.Add(new OpenApiServerDescriptor
    {
        Url = "https://api.example.com",
        Description = "Production"
    });
});

var app = builder.Build();

app.UseCoreOpenApi();
```

The application now serves:

| Route | Content |
|-------|---------|
| `/swagger/v1/swagger.json` | The patched document |
| `/swagger` | Swagger UI |
| `swagger-original/v1/swagger.json` | The unpatched generator output |

Annotate controllers with the response attributes:

```csharp
using Core.OpenApi.Attributes;

[HttpPost]
[ProducesCreated<OrderResponse>]
public async Task<IActionResult> Create(CreateOrderRequest request)
```

------------------------------------------------------------------------

## ⚙️ Problem Responses

Every configured error response is declared once in `components.responses` and
referenced from the operations that already document the status code:

```csharp
builder.Services.AddCoreOpenApi(options =>
{
    options.ProblemResponses.SchemaId = "AppProblemDetails";
    options.ProblemResponses.TypePrefix = "urn:orders:error:";
    options.ProblemResponses.TraceIdExample =
        "00-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-bbbbbbbbbbbbbbbb-01";

    // Rate limiting is applied to every endpoint, so document 429 everywhere.
    options.ProblemResponses.Definitions
        .Single(definition => definition.StatusCode == "429")
        .IncludeWhenMissing = true;
});
```

`IncludeWhenMissing` is `false` by default, so a status code is only added to
operations that already declare it. Set it to `true` only when the corresponding
middleware really applies to the whole API.

An existing `400` response is replaced only when it already looks like a problem
response (`application/problem+json`, or a schema reference to one of the
configured problem schemas). That keeps a custom validation payload untouched.

------------------------------------------------------------------------

## 🔌 Extending the Generator

Features that live in other packages stay out of this one. Use the callback:

```csharp
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

builder.Services.AddCoreOpenApi(options =>
{
    options.ConfigureSwaggerGen = gen =>
    {
        gen.EnableAnnotations();
        gen.ExampleFilters();
    };
});
```

The same pattern applies to the JSON endpoint, for example to keep it out of the
rate limiter and out of the document:

```csharp
builder.Services.AddCoreOpenApi(options =>
{
    options.ConfigureJsonEndpoint = endpoint => endpoint
        .DisableRateLimiting()
        .ExcludeFromDescription();
});
```

------------------------------------------------------------------------

## 📖 Public API

| Type | Description |
|------|-------------|
| `AddCoreOpenApi()` | Registers Swagger generation, the filters and the options. |
| `UseCoreOpenApi()` | Maps the patched document endpoint and configures Swagger UI. |
| `CoreOpenApiOptions` | Title, servers, security, XML comments and callbacks. |
| `OpenApiProblemResponsesOptions` | The shared problem response catalog. |
| `ProblemResponseDefinition` | One response: status code, component, error code and example. |
| `OpenApiDocumentPatcher` | Applies the catalog to a serialized document. |
| `JsonOnlyResponsesOperationFilter` | Removes redundant text media types. |
| `CreatedWithLocationDocumentFilter` | Documents the `Location` header on `201`. |
| `ProducesOk<T>` / `ProducesCreated<T>` / `ProducesNoContent` | Response attributes. |

------------------------------------------------------------------------

## 📚 Documentation

- Getting Started
- Configuration
- Problem Responses

------------------------------------------------------------------------

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

------------------------------------------------------------------------

## 📄 License

Released under the MIT License.
