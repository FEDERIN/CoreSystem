using Core.Cache.Abstractions;
using Core.Cache.Options;
using Microsoft.AspNetCore.Http;

namespace Core.Cache.Http.Caching;

internal sealed class HttpCacheKeyGenerator(
    CacheOptions options)
    : ICacheKeyGenerator
{
    private readonly CacheOptions _options = options;

    public string Generate(HttpContext context)
    {
        var query = context.Request.Query
            .OrderBy(q => q.Key)
            .Select(q => $"{q.Key}={q.Value}");

        return $"{_options.InstanceName ?? "cache"}:{context.Request.Path}:{string.Join("&", query)}";
    }
}