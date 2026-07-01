using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Attributes;
using Core.DistributedCache.Diagnostics;
using Core.DistributedCache.Extensions;
using Core.DistributedCache.Http;
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

        var cachedResponse =
            await _cache.GetAsync<CachedHttpResponse>(cacheKey);

        if (cachedResponse is not null)
        {
            _metrics.RecordHit();

            await RestoreResponseAsync(context, cachedResponse);

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

            var body = memoryStream.ToArray();

            string[]? tags = cacheAttribute.Tag is not null
                ? [cacheAttribute.Tag]
                : null;


            var response = new CachedHttpResponse
            {
                Body = body,
                StatusCode = context.Response.StatusCode,
                Headers = CaptureHeaders(context.Response)
            };

            await _cache.SetAsync(
                cacheKey,
                response,
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

    private static Dictionary<string, string[]> CaptureHeaders(HttpResponse response)
    {
        return response.Headers
            .Where(h => !IgnoredHeaders.Contains(h.Key))
            .ToDictionary(
                h => h.Key,
                h => h.Value
                    .Where(v => v is not null)
                    .Cast<string>()
                    .ToArray());
    }

    private static readonly HashSet<string> IgnoredHeaders =
        new(StringComparer.OrdinalIgnoreCase)
    {
        HeaderNames.TransferEncoding,
        HeaderNames.ContentLength,
        HeaderNames.Connection,
        HeaderNames.Date,
        HeaderNames.Server,
        HeaderNames.KeepAlive,
        HeaderNames.Upgrade,
        HeaderNames.TE,
        HeaderNames.Trailer,
        HeaderNames.ProxyAuthenticate,
        HeaderNames.ProxyAuthorization
    };

    private static async Task RestoreResponseAsync(
    HttpContext context,
    CachedHttpResponse response)
    {
        context.Response.StatusCode = response.StatusCode;

        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value;
        }

        if (!HttpMethods.IsHead(context.Request.Method))
        {
            context.Response.ContentLength = response.Body.Length;
            await context.Response.Body.WriteAsync(response.Body);
        }
    }
}
