# Observability

The package exposes the `Core.RateLimiting` meter through `IObservabilityContributor`. When `CoreSystem.Observability` is enabled, the meter is registered automatically.

| Metric | Type | Tags |
|---|---|---|
| `rate_limiting.rejected_requests` | Counter | `rate_limit.policy` |

Rejected requests also produce a structured warning log containing the policy name and retry delay. Client identities are intentionally not attached to metric tags, preventing high-cardinality and privacy-sensitive telemetry.

To export the metric with the standard observability package, enable metrics in the application configuration:

```json
{
  "OpenTelemetry": {
    "Metrics": {
      "Enabled": true,
      "OtlpEndpoint": "http://otel-collector:4317"
    }
  }
}
```
