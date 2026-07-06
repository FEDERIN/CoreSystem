using Core.Cache.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Core.Cache.Http.Caching;

internal sealed class HttpCacheKeyGenerator()
    : ICacheKeyGenerator
{
    public string Generate(HttpContext context)
    {
        var query = string.Join("&",
            context.Request.Query
                .OrderBy(q => q.Key)
                .Select(q => $"{q.Key}={q.Value}"));

        return string.IsNullOrEmpty(query)
            ? context.Request.Path.Value!
            : $"{context.Request.Path}?{query}";
    }
}