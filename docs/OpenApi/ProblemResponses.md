# 🧩 Problem Responses

Generated documents repeat the same error payload on every operation. The
patcher declares each one once in `components.responses` and replaces the
matching status codes with a `$ref`, so the document shrinks and generated
clients share a single error model.

## Default catalog

| Component | Status | Error code | Notes |
| --------- | ------ | ---------- | ----- |
| `Unauthorized` | 401 | `UNAUTHORIZED` | |
| `Forbidden` | 403 | `FORBIDDEN` | |
| `InternalError` | 500 | `INTERNAL_ERROR` | |
| `NotFound` | 404 | `NOT_FOUND` | |
| `Conflict` | 409 | `CONFLICT` | |
| `ValidationError` | 400 | `VALIDATION_ERROR` | Uses `ValidationProblemDetails` and adds an `errors` object. |
| `TooManyRequests` | 429 | `RATE_LIMIT_EXCEEDED` | Documents a `Retry-After` header. |

Each component references `#/components/schemas/ProblemDetails` by default and
exposes an example with `title`, `status`, `detail`, `errorCode` and the
configured extension fields.

## Customizing the catalog

```csharp
using Core.OpenApi.Options;

builder.Services.AddCoreOpenApi(options =>
{
    options.ProblemResponses.SchemaId = "OrdersProblemDetails";
    options.ProblemResponses.TypePrefix = "urn:orders:error:";
    options.ProblemResponses.TraceIdExample =
        "00-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-bbbbbbbbbbbbbbbb-01";

    options.ProblemResponses.Definitions
        .Single(definition => definition.StatusCode == "429")
        .IncludeWhenMissing = true;
});
```

| Option | Default | Description |
| ------ | ------- | ----------- |
| `Enabled` | `true` | Registers the components and rewrites the operations. |
| `SchemaId` | `ProblemDetails` | Default schema referenced by every response. |
| `ContentType` | `application/problem+json` | Media type of the generated responses. |
| `TypePrefix` | `null` | Prefix composed with the error code to build `type`. Omitted when `null`. |
| `TraceIdExample` | `null` | Value of the `traceId` extension. Omitted when `null`. |
| `ProblemSchemaIds` | `ProblemDetails`, `ValidationProblemDetails` | Schemas considered problem schemas when deciding whether to replace a response. |
| `Definitions` | see above | The response catalog. |

## Adding a response

```csharp
var definition = new ProblemResponseDefinition
{
    StatusCode = "422",
    ComponentName = "UnprocessableEntity",
    ErrorCode = "UNPROCESSABLE_ENTITY",
    Title = "Unprocessable entity",
    Description = "The request is well formed but cannot be processed.",
    Detail = "The order cannot be fulfilled with the requested ticket count.",
    SchemaId = "OrdersProblemDetails"
};

definition.AdditionalExampleFields["ticketCount"] = 0;

builder.Services.AddCoreOpenApi(options =>
    options.ProblemResponses.Definitions.Add(definition));
```

## When a response is replaced

| Mode | Behaviour |
| ---- | --------- |
| `Always` (default) | Any existing response with the status code becomes a `$ref`. |
| `WhenContentMatchesProblemDetails` | Replacement happens only when the response has no content, uses the problem content type, or references a problem schema. |
| `Never` | The component is created but operations are untouched. |

The `ValidationError` definition uses `WhenContentMatchesProblemDetails`, so a
`400` that documents a custom payload survives:

```json
"400": {
  "description": "Bad Request",
  "content": {
    "application/json": {
      "schema": { "$ref": "#/components/schemas/CustomError" }
    }
  }
}
```

An existing `$ref` is never rewritten by `WhenContentMatchesProblemDetails`, which
keeps the patcher idempotent.

## `IncludeWhenMissing`

`IncludeWhenMissing` is `false` for every default definition. A status code is
only referenced when the operation already declares it, so the document never
claims a response the API cannot produce.

Enable it only when the behaviour really applies to the whole API — rate
limiting is the usual case:

```csharp
options.ProblemResponses.Definitions
    .Single(definition => definition.StatusCode == "429")
    .IncludeWhenMissing = true;
```

## Using the patcher directly

`OpenApiDocumentPatcher` is public and does not depend on ASP.NET Core. It can
patch a document produced by another generator, or a document read from a file:

```csharp
using Core.OpenApi;
using Core.OpenApi.Options;

var patched = OpenApiDocumentPatcher.Patch(json, new OpenApiProblemResponsesOptions());
```

Invalid JSON and documents whose root is not a JSON object are returned
unchanged, so the patcher never breaks a pipeline it does not understand.
