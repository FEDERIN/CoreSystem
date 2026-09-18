# ⚡ CoreSystem.RateLimiting

![NuGet](https://img.shields.io/nuget/v/CoreSystem.RateLimiting?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.RateLimiting?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)
![OpenTelemetry](https://img.shields.io/badge/OpenTelemetry-Enabled-purple?style=for-the-badge)

`CoreSystem.RateLimiting` is a production-ready fixed-window rate-limiting package for ASP.NET Core APIs. It protects APIs with a global limiter that partitions authenticated requests by subject and anonymous requests by client IP address.

The package returns RFC 7807 Problem Details responses, exposes rejection metrics through OpenTelemetry, and remains safe behind trusted reverse proxies when the consuming API configures forwarded headers correctly.

---

# ✨ Features

- ✅ Global fixed-window rate limiter
- ✅ Partitioning by authenticated subject claim
- ✅ Remote-IP fallback for anonymous requests
- ✅ RFC 7807 `429 Too Many Requests` responses
- ✅ `Retry-After` response header when available
- ✅ Configurable permits, window, queue, and claim type
- ✅ Built-in structured rejection logging
- ✅ OpenTelemetry metrics through `IObservabilityContributor`
- ✅ Safe reverse-proxy integration guidance

---

# 📦 Installation

```bash
dotnet add package CoreSystem.RateLimiting
```

---

# 🚀 Quick Start

```csharp
using Core.RateLimiting.DependencyInjection;

builder.Services.AddCoreRateLimiting(options =>
{
    options.PermitLimit = 100;
    options.Window = TimeSpan.FromMinutes(1);
    options.QueueLimit = 0;
    options.SubjectClaimType = "sub";
});

app.UseForwardedHeaders();
app.UseAuthentication();
app.UseCoreRateLimiting();
app.UseAuthorization();
```

---

# ⚙️ Configuration

| Option | Default | Description |
|---|---:|---|
| `Enabled` | `true` | Enables the limiter registration and middleware. |
| `PolicyName` | `core-fixed-window` | Policy identifier included in logs, metrics, and Problem Details extensions. |
| `PermitLimit` | `100` | Requests allowed per partition in each window. |
| `Window` | 1 minute | Fixed-window duration. |
| `QueueLimit` | `0` | Number of requests allowed to wait for a permit. |
| `QueueProcessingOrder` | `OldestFirst` | Queue discipline when queueing is enabled. |
| `AutoReplenishment` | `true` | Enables automatic window replenishment. |
| `SubjectClaimType` | `ClaimTypes.NameIdentifier` | Claim used to identify authenticated callers. |

---

# 🔒 Client Identity and Reverse Proxies

Requests are partitioned by `subject:<claim-value>` when authenticated, otherwise by `ip:<remote-address>`.

Forwarded-header trust belongs to the consuming API because proxy addresses and networks are deployment-specific. Configure `KnownProxies` and/or `KnownNetworks`, and call `UseForwardedHeaders()` before authentication and rate limiting. Never use an arbitrary client-provided `X-Forwarded-For` value as a rate-limit identity.

---

# 🚫 Rejection Response

When the permit limit is exhausted, the middleware returns HTTP `429` with `application/problem+json`.

```json
{
  "type": "https://httpstatuses.com/429",
  "title": "Too Many Requests",
  "status": 429,
  "detail": "The request rate limit has been exceeded.",
  "policy": "management-api",
  "retryAfterSeconds": 42
}
```

`Retry-After` is also emitted when the underlying rate limiter provides retry metadata.

---

# 📊 Built-in Observability

The package publishes the `Core.RateLimiting` meter and registers it through `IObservabilityContributor` for `CoreSystem.Observability` integration.

| Metric | Type | Tags |
|---|---|---|
| `rate_limiting.rejected_requests` | Counter | `rate_limit.policy` |

Rejections are also logged as structured warnings with the policy name and retry delay. Client identities are deliberately excluded from metric tags to avoid high-cardinality and privacy-sensitive telemetry.

---

# 📚 Documentation

The complete documentation includes Getting Started, configuration, reverse-proxy safety, rejection responses, and OpenTelemetry observability.

---

# 🤝 Contributing

Contributions, bug reports, and feature requests are welcome.

---

# 📄 License

MIT License © Federin Pastor Gutierrez Ortiz
