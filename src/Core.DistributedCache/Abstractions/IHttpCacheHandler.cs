using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

internal interface IHttpCacheHandler
{
    Task HandleAsync(
        HttpContext context,
        RequestDelegate next);
}