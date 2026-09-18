# CoreSystem.RateLimiting

Limitador global fixed-window para ASP.NET Core, alineado con EventHouse: particiona por `sub` autenticado y, cuando no existe, por la IP remota. Devuelve `429` RFC 7807, agrega `Retry-After` cuando aplica y publica `rate_limiting.rejected_requests` en `Core.RateLimiting`.

```csharp
builder.Services.AddCoreRateLimiting(options => { options.PermitLimit = 100; options.Window = TimeSpan.FromMinutes(1); options.SubjectClaimType = "sub"; });
app.UseForwardedHeaders(); // KnownProxies/KnownNetworks de la API
app.UseAuthentication();
app.UseCoreRateLimiting();
app.UseAuthorization();
```

Los forwarded headers son responsabilidad de la API consumidora: deben ejecutarse antes de autenticación y rate limiting, y confiar exclusivamente en los proxies/redes reales del ingress. Nunca se acepta `X-Forwarded-For` arbitrario.
