using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.Diagnostics;

internal sealed class IdempotencyObservabilityContributor
    : IObservabilityContributor
{
    public IEnumerable<string> GetActivitySources()
        => ["Core.Idempotency"];

    public void ConfigureObservability(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddMeter(
                    IdempotencyDiagnosticsConstants.MeterName);
            });
    }
}