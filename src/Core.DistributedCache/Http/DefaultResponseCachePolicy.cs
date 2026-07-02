using Core.DistributedCache.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.DistributedCache.Http;

internal sealed class DefaultResponseCachePolicy
    : IResponseCachePolicy
{
    public bool CanCache(HttpContext context)
    {
        if (context.Response.StatusCode != StatusCodes.Status200OK)
            return false;

        if (context.Response.Headers.ContainsKey(HeaderNames.SetCookie))
            return false;

        if (context.Response.Headers.TryGetValue(
            HeaderNames.CacheControl,
            out var cacheControl))
        {
            var value = cacheControl.ToString();

            if (value.Contains("private",
                StringComparison.OrdinalIgnoreCase))
                return false;

            if (value.Contains("no-store",
                StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}