using Core.Cache.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.Cache.Http.Caching;

internal sealed class ResponseCapture
    : IResponseCapture
{
    private static readonly HashSet<string> IgnoredHeaders =
    [
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
    ];

    public async Task<CapturedResponse> CaptureAsync(
        HttpContext context,
        RequestDelegate next)
    {
        var original = context.Response.Body;

        await using var memory = new MemoryStream();

        context.Response.Body = memory;

        try
        {
            await next(context);

            return new CapturedResponse
            {
                Body = memory.ToArray(),
                StatusCode = context.Response.StatusCode,
                Headers = CaptureHeaders(context.Response)
            };
        }
        finally
        {
            memory.Position = 0;

            await memory.CopyToAsync(original);

            context.Response.Body = original;
        }
    }

    private static Dictionary<string, string[]> CaptureHeaders(
        HttpResponse response)
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
}