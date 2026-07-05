using Core.Cache.Options;
using Core.Cache.Pipeline;
using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

internal static class PipelineRegistration
{
    public static IServiceCollection AddCachePipeline(
        this IServiceCollection services,
        CacheOptions options)
    {
        services.AddSingleton<LoggingBehavior>();
        services.AddSingleton<MetricsBehavior>();

        if (options.Redis.Enabled)
        {
            services.AddSingleton<FallbackBehavior>();
            services.AddSingleton<ResilienceBehavior>();
        }

        services.AddSingleton<ICachePipeline>(sp =>
        {
            var behaviors = new List<ICacheBehavior>
            {
                sp.GetRequiredService<LoggingBehavior>(),
                sp.GetRequiredService<MetricsBehavior>()
            };

            if (options.Redis.Enabled)
            {
                behaviors.Add(sp.GetRequiredService<FallbackBehavior>());
                behaviors.Add(sp.GetRequiredService<ResilienceBehavior>());
            }

            return new CachePipeline(behaviors);
        });

        return services;
    }
}