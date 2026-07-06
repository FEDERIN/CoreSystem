using Core.Cache.Http;
using Core.Cache.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

public static class CacheRegistration
{
    public static IServiceCollection AddCoreCache(
        this IServiceCollection services,
        Action<CacheOptions> configure)
    {
        var options = new CacheOptions();
        configure(options);

        services
            .AddSingleton(options)
            .AddLogging()
            .AddCacheSerialization()
            .AddCacheDiagnostics()
            .AddCacheMemory()
            .AddCacheRedis(options);
        
        if (options.Redis.Enabled)
        {
            services.AddCacheResilience();
        }

        services
            .AddCachePipeline(options)
            .AddCacheHttp()
            .AddCacheServices();

        return services;
    }

    public static IApplicationBuilder UseCoreCache(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CacheMiddleware>();
    }
}