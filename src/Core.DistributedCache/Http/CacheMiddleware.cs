using Core.DistributedCache.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Http;

internal sealed class CacheMiddleware(
    RequestDelegate next,
    IHttpCacheHandler handler)
{
    public Task InvokeAsync(HttpContext context)
        => handler.HandleAsync(context, next);
}