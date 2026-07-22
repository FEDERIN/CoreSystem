using Core.Idempotency.Abstractions;
using Core.Idempotency.Diagnostics;
using Core.Idempotency.Models;
using Core.Idempotency.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Idempotency.Services;

internal sealed class IdempotencyService(
    IOptions<IdempotencyOptions> options,
    IdempotencyMetrics metrics,
    IIdempotencyStorage storage,
    IIdempotencyKeyResolver keyResolver)
    : IIdempotencyService
{
    private readonly IdempotencyOptions _options = options.Value;
    private readonly IdempotencyMetrics _metrics = metrics;
    private readonly IIdempotencyStorage _storage = storage;
    private readonly IIdempotencyKeyResolver _keyResolver = keyResolver;

    public async Task HandleAsync(
        HttpContext context,
        RequestDelegate next)
    {
        if (!_options.Enabled)
        {
            await next(context);
            return;
        }

        if (!_options.AllowedMethods.Contains(context.Request.Method))
        {
            await next(context);
            return;
        }

        _metrics.RecordRequest();

        if (!_keyResolver.TryResolve(
                context,
                out var key))
        {
            await next(context);
            return;
        }

        var cached =
            await _storage.GetAsync(key!);
        
        if (cached is not null)
        {
            _metrics.RecordHit();
            _metrics.RecordReplay();

            context.Response.StatusCode = cached.StatusCode;
            context.Response.ContentType = cached.ContentType;
            context.Response.Headers.Append(
                "X-Idempotency-Cache",
                "HIT");

            if (cached.StatusCode != StatusCodes.Status204NoContent &&
                !string.IsNullOrEmpty(cached.Body))
            {
                await context.Response.WriteAsync(cached.Body);
            }

            return;
        }

        _metrics.RecordMiss();

        var originalBody = context.Response.Body;

        await using var memory = new MemoryStream();

        context.Response.Body = memory;

        try
        {
            await next(context);

            if (_options.CacheableStatusCodes.Contains(
                    context.Response.StatusCode))
            {
                memory.Position = 0;

                var body =
                    await new StreamReader(memory)
                        .ReadToEndAsync();

                await _storage.SetAsync(
                    key!,
                    new IdempotencyResponse
                    {
                        StatusCode = context.Response.StatusCode,
                        ContentType = context.Response.ContentType,
                        Body = body
                    },
                    _options.Expiration);

                memory.Position = 0;
            }

            if (memory.Length > 0)
            {
                memory.Position = 0;

                await memory.CopyToAsync(originalBody);
            }
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
}