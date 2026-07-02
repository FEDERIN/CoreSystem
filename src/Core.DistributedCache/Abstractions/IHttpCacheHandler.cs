using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

public interface IHttpCacheHandler
{
    Task HandleAsync(
        HttpContext context,
        RequestDelegate next);
}