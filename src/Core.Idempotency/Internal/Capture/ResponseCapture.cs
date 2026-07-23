using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.Idempotency.Internal.Capture;

internal sealed class ResponseCapture : IAsyncDisposable
{
    private readonly HttpContext _context;
    private readonly Stream _originalBody;
    private readonly MemoryStream _memory;

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

    public ResponseCapture(HttpContext context)
    {
        _context = context;
        _originalBody = context.Response.Body;

        _memory = new MemoryStream();

        context.Response.Body = _memory;
    }

    public async Task<CapturedResponse> BuildAsync()
    {
        _memory.Position = 0;

        using var reader = new StreamReader(
            _memory,
            leaveOpen: true);

        return new CapturedResponse
        {
            StatusCode = _context.Response.StatusCode,
            ContentType = _context.Response.ContentType,
            Body = await reader.ReadToEndAsync(),
            Headers = CaptureHeaders(_context.Response)
        };
    }

    public async Task CopyToOriginalAsync()
    {
        _memory.Position = 0;

        await _memory.CopyToAsync(_originalBody);
    }

    public async ValueTask DisposeAsync()
    {
        _context.Response.Body = _originalBody;

        await _memory.DisposeAsync();
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