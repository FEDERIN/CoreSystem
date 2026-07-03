using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Http;
using Core.DistributedCache.Http.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.DependencyInjection;

internal static class HttpRegistration
{
    public static IServiceCollection AddCacheHttp(
        this IServiceCollection services)
    {
        services.AddSingleton<ICacheKeyGenerator, HttpCacheKeyGenerator>();
        services.AddSingleton<IRequestCachePolicy, DefaultRequestCachePolicy>();
        services.AddSingleton<IResponseCachePolicy, DefaultResponseCachePolicy>();
        services.AddSingleton<IResponseCapture, ResponseCapture>();
        services.AddSingleton<IHttpResponseWriter, HttpResponseWriter>();
        services.AddSingleton<IHttpCacheHandler, HttpCacheHandler>();
        return services;
    }
}