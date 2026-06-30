using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Attributes;
using Core.DistributedCache.Diagnostics;
using Core.DistributedCache.Extensions;
using Core.DistributedCache.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.DistributedCache.Middleware;

internal class CacheMiddleware(
    RequestDelegate next,
    ICoreCacheService cache,
    CacheOptions options,
    CacheMetrics metrics)
{
    private readonly RequestDelegate _next = next;
    private readonly ICoreCacheService _cache = cache;
    private readonly CacheOptions _options = options;
    private readonly CacheMetrics _metrics = metrics;

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var cacheAttribute = endpoint?.Metadata.GetMetadata<CacheableAttribute>();

        if (cacheAttribute is null)
        {
            await _next(context);
            return;
        }

        if (!CanCacheRequest(context))
        {
            await _next(context);
            return;
        }

        var expiration = cacheAttribute.ExpirationSeconds.HasValue
                ? TimeSpan.FromSeconds(cacheAttribute.ExpirationSeconds.Value)
                : _options.DefaultExpiration;

        var cacheKey = context.Request.GenerateCacheKey(_options.InstanceName ??"api_cache");

        var cachedResponse = await _cache.GetAsync<string>(cacheKey);

        if (cachedResponse is not null)
        {
            _metrics.RecordHit();

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(cachedResponse);
            return;
        }

        _metrics.RecordMiss();

        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context);

            if (!CanCacheResponse(context))
            {
                return;
            }

            if (memoryStream.Length > _options.MaxCacheableSize)
            {
                return;
            }

            memoryStream.Position = 0;
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

            string[]? tags = cacheAttribute.Tag is not null
                ? [cacheAttribute.Tag]
                : null;

            await _cache.SetAsync(
                cacheKey,
                responseBody,
                expiration,
                tags);
        }
        finally
        {
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private static bool CanCacheRequest(HttpContext context)
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

    private static bool CanCacheResponse(HttpContext context)
    {
        if (context.Response.StatusCode != StatusCodes.Status200OK)
            return false;

        if (context.Response.Headers.ContainsKey(HeaderNames.SetCookie))
            return false;

        if (context.Response.Headers.TryGetValue(HeaderNames.CacheControl, out var cacheControl))
        {
            var value = cacheControl.ToString();

            if (value.Contains("no-store", StringComparison.OrdinalIgnoreCase))
                return false;

            if (value.Contains("private", StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}
