using Microsoft.AspNetCore.Http;

namespace Core.Cache.Abstractions;

internal interface IHttpCacheHandler
{
    Task HandleAsync(
        HttpContext context,
        RequestDelegate next);
}