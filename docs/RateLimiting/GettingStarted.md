# Getting Started

Register the package during service configuration and enable its middleware after forwarded headers and authentication.

```csharp
using Core.RateLimiting.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;

builder.Services.AddCoreRateLimiting(options =>
{
    options.PermitLimit = 100;
    options.Window = TimeSpan.FromMinutes(1);
    options.QueueLimit = 0;
    options.SubjectClaimType = "sub";
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseAuthentication();
app.UseCoreRateLimiting();
app.UseAuthorization();
```

## Reverse proxies

`UseForwardedHeaders()` must be configured by the consuming API with its real `KnownProxies` and/or `KnownNetworks`. It must run before authentication and rate limiting so `HttpContext.Connection.RemoteIpAddress` reflects the client identity accepted from trusted infrastructure.

Do not trust an arbitrary `X-Forwarded-For` header from public clients.

## Rejection response

When the permit limit is exhausted, the middleware returns HTTP `429` and `application/problem+json`. The payload includes the configured policy name and, when available, `retryAfterSeconds`.
