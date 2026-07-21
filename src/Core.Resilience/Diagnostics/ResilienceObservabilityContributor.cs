
using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Resilience.Diagnostics;

internal sealed class ResilienceObservabilityContributor
    : IObservabilityContributor
{
    public IEnumerable<string> GetActivitySources()
    {
        return [ResilienceDiagnosticConstants.ActivitySourceName];
    }

    public void ConfigureObservability(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddMeter(ResilienceDiagnosticConstants.MeterName);
            });
    }
}