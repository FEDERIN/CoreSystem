using Core.DistributedCache.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Http;

internal sealed class HttpResponseWriter
    : IHttpResponseWriter
{
    public async Task WriteAsync(
        HttpContext context,
        CachedHttpResponse response)
    {
        context.Response.StatusCode = response.StatusCode;

        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value;
        }

        if (HttpMethods.IsHead(context.Request.Method))
            return;

        context.Response.ContentLength = response.Body.Length;

        await context.Response.Body.WriteAsync(response.Body);
    }
}