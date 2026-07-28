using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.Diagnostics;

internal sealed class CacheObservabilityContributor : IObservabilityContributor
{
    public IEnumerable<string> GetActivitySources()
        => [CacheDiagnosticsConstants.MeterName];

    public void ConfigureObservability(IServiceCollection services, IConfiguration configuration)
    {

        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddMeter(CacheDiagnosticsConstants.MeterName);
            });
    }
}