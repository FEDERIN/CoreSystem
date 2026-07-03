using Core.Cache.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Core.Cache.Http;

internal sealed class CacheMiddleware(
    RequestDelegate next,
    IHttpCacheHandler handler)
{
    public Task InvokeAsync(HttpContext context)
        => handler.HandleAsync(context, next);
}