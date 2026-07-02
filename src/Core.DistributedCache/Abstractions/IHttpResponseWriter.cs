using Core.DistributedCache.Http;
using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

public interface IHttpResponseWriter
{
    Task WriteAsync(
        HttpContext context,
        CachedHttpResponse response);
}