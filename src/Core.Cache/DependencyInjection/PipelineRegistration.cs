using Core.Cache.Abstractions;
using Core.Cache.Pipeline;
using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

internal static class PipelineRegistration
{
    public static IServiceCollection AddCachePipeline(
        this IServiceCollection services)
    {
        services.AddSingleton<ICachePipeline>(sp =>
        {
            var behaviors = new List<ICacheBehavior>
            {
                sp.GetRequiredService<LoggingBehavior>(),
                sp.GetRequiredService<MetricsBehavior>()
            };

            var resilience =
                sp.GetService<ResilienceBehavior>();

            if (resilience is not null)
            {
                behaviors.Add(resilience);
            }

            var resolver =
                sp.GetRequiredService<ICacheStorageResolver>();

            if (resolver.HasFallback)
            {
                behaviors.Add(
                    sp.GetRequiredService<FallbackBehavior>());
            }

            return new CachePipeline(behaviors);
        });

        return services;
    }
}