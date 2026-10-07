using Core.Resilience.Abstractions;
using Core.Resilience.Internal;
using Core.Resilience.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Resilience.DependencyInjection;

/// <summary>Registration extensions for the CoreSystem resilience pipeline.</summary>
public static class ResilienceRegistration
{
    /// <summary>Adds CoreSystem resilience services configured via a delegate.</summary>
    public static IServiceCollection AddCoreResilience(
        this IServiceCollection services,
        Action<ResilienceOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new ResilienceOptions();

        configure(options);

        services.AddSingleton(options);

        if (!options.Enabled)
        {
            services.AddSingleton<
                IResiliencePipelineProvider,
                NoOpResiliencePipelineProvider>();

            return services;
        }



        services.AddDiagnostics();
        services.AddPipelineServices();
        services.AddStrategies();

        return services;
    }
}