using Core.Http.Abstractions;
using Core.Http.Responses;
using Core.Idempotency.Abstractions;
using Core.Idempotency.Constants;
using Core.Idempotency.Diagnostics;
using Core.Idempotency.Exceptions;
using Core.Idempotency.Fingerprint;
using Core.Idempotency.Internal;
using Core.Idempotency.Models;
using Core.Idempotency.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Idempotency.Services;

internal sealed class IdempotencyService(
    IOptions<IdempotencyOptions> options,
    IdempotencyMetrics metrics,
    IIdempotencyStorage storage,
    IIdempotencyKeyResolver keyResolver,
    IRequestFingerprintProvider fingerprintProvider,
    IResponseCapture responseCapture,
    IHttpResponseWriter responseWriter)
    : IIdempotencyService
{
    private readonly IdempotencyOptions _options = options.Value;
    private readonly IdempotencyMetrics _metrics = metrics;
    private readonly IIdempotencyStorage _storage = storage;
    private readonly IIdempotencyKeyResolver _keyResolver = keyResolver;
    private readonly IRequestFingerprintProvider _fingerprintProvider = fingerprintProvider;
    private readonly IResponseCapture _responseCapture = responseCapture;
    private readonly IHttpResponseWriter _responseWriter = responseWriter;

    public async Task HandleAsync(
        HttpContext context,
        RequestDelegate next,
        CancellationToken cancellationToken = default)
    {
        var request = ResolveRequest(context);

        if (request is null)
        {
            await next(context);
            return;
        }

        RequestFingerprint? requestFingerprint = null;

        if (_options.Fingerprint.Enabled)
        {
            requestFingerprint =
                await _fingerprintProvider.ComputeAsync(
                    context,
                    cancellationToken);
        }

        _metrics.RecordRequest();

        if (await ResolveAsync(
                context,
                request,
                requestFingerprint,
                cancellationToken))
        {
            return;
        }

        _metrics.RecordMiss();

        await ExecuteRequestAsync(
            context,
            request,
            requestFingerprint,
            next,
            cancellationToken);
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

    private async Task<bool> ResolveAsync(
        HttpContext context,
        IdempotencyContext request,
        RequestFingerprint? requestFingerprint,
        CancellationToken cancellationToken = default)
    {
        var entry = await _storage.GetAsync(request.Key, cancellationToken);

        if (entry is null)
        {
            return false;
        }

        if (entry.RequestFingerprint is not null &&
            requestFingerprint is not null &&
            entry.RequestFingerprint != requestFingerprint)
        {
            throw new IdempotencyFingerprintMismatchException();
        }


        _metrics.RecordHit();
        _metrics.RecordReplay();

        await ReplayResponseAsync(
            context,
            entry.Response,
            cancellationToken);

        return true;
    }

    private async Task ExecuteRequestAsync(
        HttpContext context,
        IdempotencyContext request,
        RequestFingerprint? requestFingerprint,
        RequestDelegate next,
        CancellationToken cancellationToken = default)
    {
        var response =
            await _responseCapture.CaptureAsync(
                context,
                next,
                cancellationToken);

        if (!_options.CacheableStatusCodes.Contains(response.StatusCode))
        {
            return;
        }

        await PersistResponseAsync(
            request,
            response,
            requestFingerprint,
            cancellationToken);
    }

    private async Task PersistResponseAsync(
        IdempotencyContext request,
        CapturedResponse response,
        RequestFingerprint? requestFingerprint,
        CancellationToken cancellationToken = default)
    {
        await _storage.SetAsync(
            request.Key,
            new IdempotencyEntry
            {
                RequestFingerprint = requestFingerprint,
                Response = new IdempotencyResponse
                {
                    StatusCode = response.StatusCode,
                    ContentType = response.ContentType,
                    Body = response.Body,
                    Headers = response.Headers
                }
            },
            request.Expiration,
            cancellationToken);
    }

    private async Task ReplayResponseAsync(
        HttpContext context,
        IdempotencyResponse cached,
        CancellationToken cancellationToken = default)
    {
        context.Response.Headers.Append(
            HeaderNames.IdempotencyCache,
            HeaderValues.Hit);

        await _responseWriter.WriteAsync(
            context,
            new CapturedResponse
            {
                StatusCode = cached.StatusCode,
                Body = cached.Body,
                ContentType = cached.ContentType,
                Headers = cached.Headers,
            },
            cancellationToken);
    }
}