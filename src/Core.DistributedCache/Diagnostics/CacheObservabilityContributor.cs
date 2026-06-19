using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.Diagnostics;

public class CacheObservabilityContributor : IObservabilityContributor
{
    public IEnumerable<string> GetActivitySources()
        => ["Core.DistributedCache"];

    public void ConfigureObservability(IServiceCollection services, IConfiguration configuration)
    {

        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddMeter(CacheDiagnosticsConstants.MeterName);
            });
    }
}