# CoreSystem.RateLimiting

`CoreSystem.RateLimiting` provides a global fixed-window rate limiter for ASP.NET Core APIs.

It follows the EventHouse identity model:

- Use the authenticated subject claim as the partition key.
- Fall back to the client remote IP address when the request is anonymous.
- Return RFC 7807 `ProblemDetails` with HTTP `429` when a request is rejected.
- Include `Retry-After` when the underlying limiter can provide it.

The package is deliberately policy-light. Each API owns its permit limits, window, claim name, endpoint exposure, and trusted reverse-proxy configuration.

## Package

```bash
dotnet add package CoreSystem.RateLimiting
```

## Main API

```csharp
builder.Services.AddCoreRateLimiting(options =>
{
    options.PermitLimit = 100;
    options.Window = TimeSpan.FromMinutes(1);
    options.SubjectClaimType = "sub";
});

app.UseCoreRateLimiting();
```

See [Getting Started](GettingStarted.md) for middleware order and proxy safety.
