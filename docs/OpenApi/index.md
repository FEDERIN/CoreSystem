# 🧭 CoreSystem.OpenApi

> **Generate the OpenAPI document once. Reuse the conventions everywhere.**

CoreSystem.OpenApi provides the OpenAPI and Swagger infrastructure shared by
CoreSystem-based APIs. It owns the document wiring, the response attributes and
the filters; each application keeps its own title, servers, examples and error
vocabulary.

The package targets .NET 8, .NET 9 and .NET 10 and depends only on
`Swashbuckle.AspNetCore.SwaggerGen` and `Swashbuckle.AspNetCore.SwaggerUI`.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.OpenApi?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.OpenApi?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-blue?style=for-the-badge)

---

## 📚 Table of Contents

- 🚀 [Getting Started](./GettingStarted.md)
- ⚙️ [Configuration](./Configuration.md)
- 🧩 [Problem Responses](./ProblemResponses.md)

---

## What the package owns

| Concern | Where it lives |
| ------- | -------------- |
| Swagger generation and UI wiring | `AddCoreOpenApi` / `UseCoreOpenApi` |
| Bearer security scheme | `CoreOpenApiOptions.Bearer` |
| Response attributes | `ProducesOk<T>`, `ProducesCreated<T>`, `ProducesNoContent` |
| Document filters | `CreatedWithLocationDocumentFilter`, `JsonOnlyResponsesOperationFilter` |
| Shared error components | `OpenApiDocumentPatcher` and `OpenApiProblemResponsesOptions` |

## What the application keeps

The title, the description, the server list, the XML documentation paths, the
request and response examples, and the error codes of its own domain. Examples
implement `IExamplesProvider<T>` from `Swashbuckle.AspNetCore.Filters` and stay
in the API project, because they are bound to its contracts.

---

## Position in the ecosystem

```text
ASP.NET Core application
        │
        ├── CoreSystem.OpenApi        (document + shared error components)
        ├── CoreSystem.Http.ProblemDetails  (the error contract itself)
        └── Swashbuckle.AspNetCore    (generation, UI)
```

The package never references the application, and the application is free to
swap Swagger UI for another renderer: the patched document is plain JSON.
