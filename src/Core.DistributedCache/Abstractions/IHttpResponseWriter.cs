using Core.DistributedCache.Http.Caching;
using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

internal interface IHttpResponseWriter
{
    Task WriteAsync(
        HttpContext context,
        CachedHttpResponse response);
}