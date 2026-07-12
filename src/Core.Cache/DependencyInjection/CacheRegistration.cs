using Core.Cache.Abstractions;
using Core.Cache.Http;
using Core.Cache.Options;
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
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new CacheOptions();
        configure(options);

        services
            .AddSingleton<CacheOptions>(options)
            .AddLogging()
            .AddCoreSerialization(serialization =>
            {
                serialization.DefaultSerializer = options.SerializerType;
            })
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
        ArgumentNullException.ThrowIfNull(app);

        if (app.ApplicationServices.GetService<IHttpCacheHandler>() is null)
        {
            throw new InvalidOperationException(CacheMessages.MissingRegistration);
        }

        return app.UseMiddleware<CacheMiddleware>();
    }
}