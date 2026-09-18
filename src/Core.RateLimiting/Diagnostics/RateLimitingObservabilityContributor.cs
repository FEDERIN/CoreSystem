using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.RateLimiting.Diagnostics;

/// <summary>
/// Registers rate-limiting metrics when the host uses CoreSystem.Observability.
/// </summary>
internal sealed class RateLimitingObservabilityContributor
    : IObservabilityContributor
{
    public IEnumerable<string> GetActivitySources()
        => [RateLimitingMetrics.MeterName];

    public void ConfigureObservability(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics.AddMeter(RateLimitingMetrics.MeterName));
    }
}
