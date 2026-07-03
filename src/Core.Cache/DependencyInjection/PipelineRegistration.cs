using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Behaviors;
using Core.Cache.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

internal static class PipelineRegistration
{
    public static IServiceCollection AddCachePipeline(
        this IServiceCollection services)
    {
        services.AddSingleton<LoggingBehavior>();
        services.AddSingleton<MetricsBehavior>();
        services.AddSingleton<FallbackBehavior>();
        services.AddSingleton<ResilienceBehavior>();

        services.AddSingleton<ICachePipeline>(sp =>
            new CachePipeline(
            [
                sp.GetRequiredService<LoggingBehavior>(),
                sp.GetRequiredService<MetricsBehavior>(),
                sp.GetRequiredService<FallbackBehavior>(),
                sp.GetRequiredService<ResilienceBehavior>()
            ]));

        return services;
    }
}
