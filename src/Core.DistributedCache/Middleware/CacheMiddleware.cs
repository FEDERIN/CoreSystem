using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Attributes;
using Core.DistributedCache.Diagnostics;
using Core.DistributedCache.Extensions;
using Core.DistributedCache.Options;
using Microsoft.AspNetCore.Http;

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
    
        if (cacheAttribute == null)
        {
            await _next(context);
            return;
        }

        var expiration = cacheAttribute.ExpirationSeconds.HasValue
                ? TimeSpan.FromSeconds(cacheAttribute.ExpirationSeconds.Value)
                : _options.DefaultExpiration;

        var cacheKey = context.Request.GenerateCacheKey(_options.InstanceName ??"api_cache");

        var cachedResponse = await _cache.GetAsync<string>(cacheKey);

        if (cachedResponse != null)
        {
            _metrics.RecordHit();
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

            if (context.Response.StatusCode == StatusCodes.Status200OK && memoryStream.Length <= _options.MaxCacheableSize)
            {
                memoryStream.Position = 0;
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                string[]? tags = cacheAttribute.Tag != null ? [cacheAttribute.Tag] : null;

                await _cache.SetAsync(cacheKey, responseBody, expiration);

                await _cache.SetAsync(
                    cacheKey,
                    responseBody,
                    expiration,
                    tags);
            }
        }
        finally
        {
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }
}
