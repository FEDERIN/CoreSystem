# 🚀 Getting Started

## 1. Install

```bash
dotnet add package CoreSystem.OpenApi
```

## 2. Register the document

```csharp
using Core.OpenApi.DependencyInjection;
using Core.OpenApi.Options;

builder.Services.AddCoreOpenApi(options =>
{
    options.Title = "Orders API";
    options.Description = "Order management endpoints";
    options.Version = "v1";

    options.Servers.Add(new OpenApiServerDescriptor
    {
        Url = "https://api.example.com",
        Description = "Production"
    });
});
```

`AddCoreOpenApi` registers Swagger generation, the CoreSystem filters, the
options instance and an in-memory cache for the patched document. It also
registers an endpoints API explorer, which is required by the generator.

## 3. Map the endpoint

Call `UseCoreOpenApi` after the application is built. The method maps the JSON
endpoint and configures Swagger UI:

```csharp
using Core.OpenApi.Extensions;

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCoreOpenApi();

app.Run();
```

Three routes become available:

| Route | Content |
| ----- | ------- |
| `/swagger/v1/swagger.json` | The patched document, served from the cache |
| `/swagger` | Swagger UI |
| `swagger-original/v1/swagger.json` | The unpatched generator output, for diagnostics |

## 4. Annotate controllers

```csharp
using Core.OpenApi.Attributes;

[ApiController]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesOk<OrderResponse>]
    public ActionResult<OrderResponse> Get(Guid id) => Ok(/* ... */);

    [HttpPost]
    [ProducesCreated<OrderResponse>]
    public ActionResult Create(CreateOrderRequest request) => CreatedAtAction(/* ... */);

    [HttpDelete("{id:guid}")]
    [ProducesNoContent]
    public IActionResult Delete(Guid id) => NoContent();
}
```

`ProducesCreated<T>` documents the `201` response, and
`CreatedWithLocationDocumentFilter` adds the `Location` header to it
automatically.

## 5. Verify

```bash
curl -s http://localhost:5185/swagger/v1/swagger.json | jq '.components.responses | keys'
```

The command lists the shared responses, for example:

```json
["Conflict", "Forbidden", "InternalError", "NotFound", "TooManyRequests", "Unauthorized", "ValidationError"]
```

## Minimal setup

When the defaults are enough, both calls need no arguments:

```csharp
builder.Services.AddCoreOpenApi();
var app = builder.Build();
app.UseCoreOpenApi();
```

The title then falls back to the entry assembly name, and the document uses the
generic problem response catalog.
