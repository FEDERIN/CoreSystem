using Core.Cache.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.Cache.Http.Caching;

internal sealed class DefaultRequestCachePolicy
    : IRequestCachePolicy
{
    public bool CanCache(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method) &&
            !HttpMethods.IsHead(context.Request.Method))
        {
            return false;
        }

        if (context.Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
            return false;
        }

        return true;
    }
}