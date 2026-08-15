# 🩺 Health Checks

CoreSystem.Cache is designed to integrate with the ASP.NET Core Health Checks infrastructure.

The health check can be used to expose the operational state of the cache layer, including the state of the primary provider when an external provider and fallback storage are configured.

---

# Why It Matters

In production environments, an external cache provider can become temporarily unavailable.

When fallback support is configured, the framework can switch the current cache operation to the fallback provider while keeping the application running.

A health check can expose this state so monitoring systems can distinguish between:

- A healthy cache infrastructure.
- A degraded cache infrastructure operating with the fallback provider.

---

# Registering Health Checks

Register the ASP.NET Core Health Checks service in the application.

```csharp
builder.Services.AddHealthChecks();
```

The current `CoreSystem.Cache` source provided for this review does not contain a health-check registration implementation, so the automatic registration described in the previous version of this document cannot be confirmed from the available `CoreSystem.Cache` code.

---

# Expose the Health Endpoint

Expose the ASP.NET Core health endpoint as usual.

```csharp
app.MapHealthChecks("/health");
```

Example:

```text
GET /health
```

---

# Health States

When a health-check implementation reports the cache provider state, the expected operational distinction is:

| Status | Description |
|---------|-------------|
| 🟢 Healthy | The primary cache provider is available. |
| 🟡 Degraded | The primary provider is unavailable and the fallback provider is being used. |

The current `CoreSystem.Cache` code contains `IPrimaryHealthStateWriter` and `FallbackBehavior`, which supports this model, but the health-check implementation itself was not included in the source reviewed here.

---

# Fallback State

When the primary storage fails and a fallback provider exists, `FallbackBehavior`:

- Marks the primary storage as unavailable.
- Changes the current cache context to the fallback storage.
- Marks the operation with `CacheEntryOptions.Rehydrate`.
- Executes the operation using the fallback storage.

This state can be used by a health-check implementation to report a degraded cache condition.

---

# Cache Rehydration

When fallback operations are marked with:

```csharp
CacheEntryOptions.Rehydrate
```

the cache entry is prepared for rehydration by the recovery components.

The current `CoreSystem.Cache` source defines the rehydration option and tracking support, but the complete rehydration service is provided outside the core implementation reviewed here.

---

# Monitoring

The ASP.NET Core health endpoint can be consumed by monitoring and orchestration systems that support Health Checks.

The exact health-check response and provider-state reporting depend on the health-check implementation registered by the application or the corresponding external package.

---

# Operational Recommendations

## Healthy

The primary cache provider is operating normally.

---

## Degraded

The primary provider is unavailable and cache operations are using the configured fallback provider.

Recommended actions:

- Verify the external cache provider availability.
- Review provider logs.
- Check network connectivity.
- Check provider authentication and configuration.

Once the primary provider becomes available again, the recovery components can rehydrate entries marked for recovery.

---

# Best Practices

- Expose a health endpoint for production applications.
- Monitor degraded states instead of only failures.
- Combine Health Checks with OpenTelemetry metrics.
- Use readiness probes when deploying to orchestration platforms.
- Configure alerts for prolonged degraded states.
