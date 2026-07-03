using Core.Cache.Http.Caching;
using Microsoft.AspNetCore.Http;

namespace Core.Cache.Abstractions;

internal interface IHttpResponseWriter
{
    Task WriteAsync(
        HttpContext context,
        CachedHttpResponse response);
}