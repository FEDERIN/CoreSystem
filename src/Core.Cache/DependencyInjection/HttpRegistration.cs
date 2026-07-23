using Core.Cache.Abstractions;
using Core.Cache.Http;
using Core.Cache.Http.Caching;
using Core.Http.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

internal static class HttpRegistration
{
    public static IServiceCollection AddCacheHttp(
        this IServiceCollection services)
    {
        services.AddSingleton<ICacheKeyGenerator, HttpCacheKeyGenerator>();
        services.AddSingleton<IRequestCachePolicy, DefaultRequestCachePolicy>();
        services.AddSingleton<IResponseCachePolicy, DefaultResponseCachePolicy>();
        services.AddCoreHttp();
        services.AddSingleton<IHttpCacheHandler, HttpCacheHandler>();
        return services;
    }
}