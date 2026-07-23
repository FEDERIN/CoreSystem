using Core.Idempotency.Abstractions;
using Core.Idempotency.Constants;
using Core.Idempotency.Diagnostics;
using Core.Idempotency.Internal;
using Core.Idempotency.Internal.Capture;
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
        var request = ResolveRequest(context);

        if (request is null)
        {
            await next(context);
            return;
        }

        _metrics.RecordRequest();

        if (await TryReplayAsync(context, request))
        {
            return;
        }

        _metrics.RecordMiss();

        await ExecuteRequestAsync(
            context,
            request,
            next);
    }

    private IdempotencyContext? ResolveRequest(
        HttpContext context)
    {
        if (!_options.Enabled)
        {
            return null;
        }

        if (!_options.AllowedMethods.Contains(context.Request.Method))
        {
            return null;
        }

        if (!_keyResolver.TryResolve(context, out var key))
        {
            return null;
        }

        return new IdempotencyContext
        {
            Key = key!,
            Expiration = _options.Expiration
        };
    }

    private async Task<bool> TryReplayAsync(
        HttpContext context,
        IdempotencyContext request)
    {
        var cached = await _storage.GetAsync(request.Key);

        if (cached is null)
        {
            return false;
        }

        _metrics.RecordHit();
        _metrics.RecordReplay();

        await ReplayResponseAsync(
            context,
            cached);

        return true;
    }

    private async Task ExecuteRequestAsync(
    HttpContext context,
    IdempotencyContext request,
    RequestDelegate next)
    {
        await using var capture =
            new ResponseCapture(context);

        await next(context);

        if (!_options.CacheableStatusCodes.Contains(
                context.Response.StatusCode))
        {
            return;
        }

        var response =
            await capture.BuildAsync();

        await PersistResponseAsync(
            request,
            response);

        await capture.CopyToOriginalAsync();
    }

    private Task PersistResponseAsync(
        IdempotencyContext request,
        CapturedResponse response)
    {
        return _storage.SetAsync(
            request.Key,
            new IdempotencyResponse
            {
                StatusCode = response.StatusCode,
                ContentType = response.ContentType,
                Body = response.Body
            },
            request.Expiration);
    }

    private static async Task ReplayResponseAsync(
    HttpContext context,
    IdempotencyResponse cached)
    {
        context.Response.StatusCode = cached.StatusCode;
        context.Response.ContentType = cached.ContentType;

        context.Response.Headers.Append(
            HeaderNames.IdempotencyCache,
            HeaderValues.Hit);

        if (cached.StatusCode != StatusCodes.Status204NoContent &&
            !string.IsNullOrEmpty(cached.Body))
        {
            await context.Response.WriteAsync(cached.Body);
        }
    }
}