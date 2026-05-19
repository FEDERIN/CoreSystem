using Core.Idempotency.Models;
using Core.Idempotency.Storage;
using Core.Idempotency.Options;
using Core.Idempotency.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Idempotency.Middleware;

/// <summary>
/// Middleware responsible for handling request idempotency by intercepting
/// specific HTTP methods and caching successful responses.
/// </summary>
public class IdempotencyMiddleware(
    RequestDelegate next,
    IIdempotencyStorage storage,
    IOptions<IdempotencyOptions> options,
    IdempotencyMetrics metrics)
{
    private readonly RequestDelegate _next = next;
    private readonly IIdempotencyStorage _storage = storage;
    private readonly IdempotencyOptions _options = options.Value;
    private readonly IdempotencyMetrics _metrics = metrics;

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. New check: If disabled, bypass the entire middleware
        if (!_options.Enabled)
        {
            await _next(context);
            return;
        }

        // 2. Validate if the HTTP method is allowed for idempotency
        if (!_options.AllowedMethods.Contains(context.Request.Method))
        {
            await _next(context);
            return;
        }

        // 3. Try to extract the idempotency key from headers
        if (!context.Request.Headers.TryGetValue(_options.HeaderName, out var key) || string.IsNullOrEmpty(key))
        {
            await _next(context);
            return;
        }

        // 4. Check if the response is already present in storage
        var cached = await _storage.GetAsync(key!);

        if (cached != null)
        {
            // Record hit in metrics and inform the client via headers
            _metrics.RecordHit();

            context.Response.StatusCode = cached.StatusCode;
            context.Response.ContentType = cached.ContentType;
            context.Response.Headers.Append("X-Idempotency-Cache", "HIT");

            await context.Response.WriteAsync(cached.Body ?? string.Empty);
            return;
        }

        // 5. Record a miss and capture the response to save it later
        _metrics.RecordMiss();

        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context);

            // 6. Save to storage only if the status code indicates success (2xx)
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                memoryStream.Position = 0;
                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                await _storage.SaveAsync(key!, new IdempotencyResponse
                {
                    StatusCode = context.Response.StatusCode,
                    ContentType = context.Response.ContentType,
                    Body = responseBody
                }, _options.Expiration);

                memoryStream.Position = 0;
            }

            await memoryStream.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}