using Core.Cache.Abstractions;
using Core.Cache.Attributes;
using Core.Cache.Http.Caching;
using Core.Cache.Options;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Core.Cache.Http;

internal sealed class HttpCacheHandler(
    ICoreCacheService cache,
    CacheOptions options,
    IRequestCachePolicy requestPolicy,
    IResponseCachePolicy responsePolicy,
    ICacheKeyGenerator keyGenerator,
    IResponseCapture responseCapture,
    IHttpResponseWriter responseWriter)
    : IHttpCacheHandler
{
    public async Task HandleAsync(
        HttpContext context,
        RequestDelegate next)
    {
        var attribute = context.GetEndpoint()?
            .Metadata
            .GetMetadata<CacheableAttribute>();

        if (attribute is null)
        {
            await next(context);
            return;
        }

        if (!requestPolicy.CanCache(context))
        {
            await next(context);
            return;
        }

        var key = keyGenerator.Generate(context);

        var sw = Stopwatch.StartNew();

        var cached =
            await cache.GetAsync<CachedHttpResponse>(key);

        Console.WriteLine($"GetAsync: {sw.ElapsedMilliseconds} ms");

        if (cached is not null)
        {
            await responseWriter.WriteAsync(context, cached);

            Console.WriteLine($"WriteAsync: {sw.ElapsedMilliseconds} ms");

            return;
        }

        var response =
            await responseCapture.CaptureAsync(context, next);

        if (!responsePolicy.CanCache(context))
            return;

        var expiration = attribute.ExpirationSeconds.HasValue
            ? TimeSpan.FromSeconds(attribute.ExpirationSeconds.Value)
            : options.DefaultExpiration;

        string[]? tags =
            attribute.Tag is not null
                ? [attribute.Tag]
                : null;

        await cache.SetAsync(
            key,
            new CachedHttpResponse
            {
                Body = response.Body,
                StatusCode = response.StatusCode,
                Headers = response.Headers
            },
            expiration,
            tags);
    }
}