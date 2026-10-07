# CoreSystem.Correlation

Correlation ID propagation middleware for ASP.NET Core — usable standalone or alongside [CoreSystem.Observability](https://federin.github.io/CoreSystem/Observability).

## Features

- Honors a client-provided `X-Correlation-Id` header (configurable name), generating a new one otherwise
- Exposes the current ID via `HttpContext.Items[CoreCorrelationConstants.HttpContextItemKey]`
- Echoes the ID back in the response headers
- Enriches log scopes with a `correlationId` property (works with `ILogger` providers)
- Fail-safe, overwrite-safe input validation (length + character allowlist)

## Installation

```bash
dotnet add package CoreSystem.Correlation
```

## Usage

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreCorrelation(); // or builder.Services.AddCoreCorrelation(options => ...);

var app = builder.Build();

app.UseCoreCorrelationId(); // after UseRouting/UseCors, before endpoints

// Access the current ID anywhere in the pipeline:
app.MapGet("/ping", (HttpContext ctx) =>
    Results.Ok(ctx.Items[CoreCorrelationConstants.HttpContextItemKey]));

app.Run();
```

### Options

| Property | Default | Description |
| --- | --- | --- |
| `Enabled` | `true` | Enables correlation propagation |
| `HeaderName` | `X-Correlation-Id` | Header read from (and written back to) the request |
| `IncludeInResponse` | `true` | Whether the ID is echoed in the response |
| `MaximumLength` | `128` | Maximum accepted length for a client ID |

## License

MIT License © Federin Pastor Gutierrez Ortiz
