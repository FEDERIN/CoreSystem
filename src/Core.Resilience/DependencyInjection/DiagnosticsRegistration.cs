

using Core.Observability.Abstractions;
using Core.Resilience.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Resilience.DependencyInjection;

internal static class DiagnosticsRegistration
{
    public static IServiceCollection AddDiagnostics(
        this IServiceCollection services)
    {
        services.AddMetrics();

        services.AddSingleton<ResilienceMetrics>();

        services.AddSingleton<IObservabilityContributor,
            ResilienceObservabilityContributor>();

        return services;
    }
}