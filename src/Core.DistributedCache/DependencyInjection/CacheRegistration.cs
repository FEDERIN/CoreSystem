using Core.Cache.Http;
using Core.Cache.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

public static class CacheRegistration
{
    public static IServiceCollection AddCoreDistributedCache(
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
            .AddCacheRedis(options)
            .AddCacheResilience()
            .AddCachePipeline()
            .AddCacheHttp()
            .AddCacheServices();

        return services;
    }

    public static IApplicationBuilder UseCoreDistributedCache(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CacheMiddleware>();
    }
}