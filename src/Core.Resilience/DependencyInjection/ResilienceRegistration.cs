using Core.Resilience.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Resilience.DependencyInjection;

/// <summary>
/// Provides extension methods for registering Core.Resilience.
/// </summary>
public static class ResilienceRegistration
{
    /// <summary>
    /// Registers Core.Resilience services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configures resilience pipelines.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCoreResilience(
        this IServiceCollection services,
        Action<ResilienceOptions>? configure = null)
    {
        services.AddOptions<ResilienceOptions>();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.AddDiagnostics();
        services.AddPipelineServices();
        services.AddStrategies();
        return services;
    }
}