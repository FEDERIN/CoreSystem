# ⚙️ Configuration

Every setting lives in `CoreOpenApiOptions`, passed to `AddCoreOpenApi`.

## Document

| Option | Default | Description |
| ------ | ------- | ----------- |
| `DocumentName` | `v1` | Document identifier used by the generator and the UI. |
| `Title` | Entry assembly name | API title. |
| `Description` | `null` | Optional API description. |
| `Version` | `v1` | Value of `info.version`. |
| `Servers` | empty | Server entries rendered by Swagger UI. |

## Routes

| Option | Default | Description |
| ------ | ------- | ----------- |
| `SwaggerRoutePrefix` | `swagger` | Route prefix of Swagger UI. |
| `JsonEndpointRoute` | `null` | Absolute route of the patched document. When `null`, it is `/{SwaggerRoutePrefix}/{DocumentName}/swagger.json`. |
| `OriginalJsonRouteTemplate` | `null` | Route template of the unpatched document. When `null`, it is `{SwaggerRoutePrefix}-original/{documentName}/swagger.json`. Set an empty string to stop exposing it. |

Deriving the routes from the document name avoids the classic mismatch where the
UI points at a document that the generator never produced:

```csharp
builder.Services.AddCoreOpenApi(options =>
{
    options.DocumentName = "v2";
    options.SwaggerRoutePrefix = "docs";

    // /docs/v2/swagger.json  and  docs-original/{documentName}/swagger.json
});
```

## Security

| Option | Default | Description |
| ------ | ------- | ----------- |
| `Bearer.Enabled` | `true` | Registers the scheme and applies it to every operation. |
| `Bearer.SchemeName` | `bearer` | Component key of the security scheme. |
| `Bearer.HeaderName` | `Authorization` | Header carrying the token. |
| `Bearer.Scheme` | `bearer` | HTTP authentication scheme. |
| `Bearer.BearerFormat` | `JWT` | Format hint. |
| `Bearer.Description` | `JWT Authorization header. Example: Bearer {token}` | Help text in the UI. |

Set `Bearer.Enabled = false` for anonymous APIs.

## XML documentation

| Option | Default | Description |
| ------ | ------- | ----------- |
| `XmlCommentFiles` | empty | Explicit XML documentation paths. |
| `IncludeControllerXmlComments` | `true` | Includes controller summaries and remarks. |

When `XmlCommentFiles` is empty, every `*.xml` file in the application base
directory is included. That covers the API assembly and its dependencies without
hard-coding a file name:

```csharp
builder.Services.AddCoreOpenApi(options =>
{
    options.XmlCommentFiles.Add(Path.Combine(
        AppContext.BaseDirectory,
        "Orders.Api.xml"));
});
```

## Behaviour

| Option | Default | Description |
| ------ | ------- | ----------- |
| `EnableDocumentPatch` | `true` | Applies `OpenApiDocumentPatcher` before serving the document. |
| `CachePatchedDocument` | `true` | Serializes and patches once per application lifetime. |
| `ConfigureSwaggerGen` | `null` | Callback for features owned by other packages. |
| `ConfigureJsonEndpoint` | `null` | Callback for the endpoint that serves the document. |

### Extending the generator

Annotations and example filters are opt-in, so the package keeps its dependency
graph small:

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

### Excluding the endpoint

The JSON endpoint is mapped as a normal GET endpoint. Keep it out of the rate
limiter and out of the document:

```csharp
builder.Services.AddCoreOpenApi(options =>
{
    options.ConfigureJsonEndpoint = endpoint => endpoint
        .DisableRateLimiting()
        .ExcludeFromDescription();
});
```

## Validation

`AddCoreOpenApi` validates the configuration at startup and throws
`ArgumentException` when the document name, the route prefix or the version is
empty, or when a problem response definition misses its status code, component
name or error code.
