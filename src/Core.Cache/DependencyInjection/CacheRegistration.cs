using Core.Cache.Abstractions;
using Core.Cache.Http;
using Core.Cache.Options;
using Core.Cache.Pipeline.Behaviors;
using Core.Cache.Services;
using Core.Serialization.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

public static class CacheRegistration
{
    public static IServiceCollection AddCoreCache(
        this IServiceCollection services,
        Action<CacheOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new CacheOptions();
        configure(options);

        services.AddSingleton(options);

        if(!options.Enabled)
        {
            services.AddSingleton<ICoreCache, NoOpCoreCache>();
            return services;
        }

        services
            .AddLogging()
            .AddCoreSerialization(serialization =>
            {
                serialization.DefaultSerializer = options.SerializerType;
            })
            .AddCacheDiagnostics()
            .AddCacheMemory();

        services
            .AddSingleton<LoggingBehavior>()
            .AddSingleton<MetricsBehavior>()
            .AddSingleton<FallbackBehavior>()
            .AddCachePipeline()
            .AddCacheHttp()
            .AddCacheServices();

        return services;
    }

    public static IApplicationBuilder UseCoreCache(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetService<CacheOptions>()
            ?? throw new InvalidOperationException(
                CacheMessages.MissingRegistration);

        if (!options.Enabled)
        {
            return app;
        }

        if (app.ApplicationServices.GetService<IHttpCacheHandler>() is null)
        {
            throw new InvalidOperationException(CacheMessages.MissingRegistration);
        }

        return app.UseMiddleware<CacheMiddleware>();
    }
}