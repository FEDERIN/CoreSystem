# Configuration

Configure the global fixed-window limiter through `RateLimitingOptions`.

| Option | Default | Description |
|---|---:|---|
| `Enabled` | `true` | Enables the registration and middleware. |
| `PolicyName` | `core-fixed-window` | Identifies the policy in logs, metrics, and response extensions. |
| `PermitLimit` | `100` | Requests allowed per partition and window. |
| `Window` | 1 minute | Fixed-window duration. |
| `QueueLimit` | `0` | Number of requests that may wait for a permit. |
| `QueueProcessingOrder` | `OldestFirst` | Queue discipline when queuing is enabled. |
| `AutoReplenishment` | `true` | Automatically opens the next fixed window. |
| `SubjectClaimType` | `ClaimTypes.NameIdentifier` | Claim used as the authenticated partition key. |

```csharp
builder.Services.AddCoreRateLimiting(options =>
{
    options.PolicyName = "management-api";
    options.PermitLimit = 60;
    options.Window = TimeSpan.FromMinutes(1);
    options.QueueLimit = 0;
    options.SubjectClaimType = "sub";
});
```

For authenticated requests, the key is `subject:<claim value>`. Anonymous requests use `ip:<remote address>`.
