using Core.Http.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Core.Http.Responses;

internal sealed class ResponseCapture
    : IResponseCapture
{
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
            context.Response.Body = original;

            memory.Position = 0;

            await memory.CopyToAsync(original);
        }
    }

    private static Dictionary<string, string[]> CaptureHeaders(
        HttpResponse response)
    {
        return response.Headers
            .Where(h => !IgnoredResponseHeaders.Values.Contains(h.Key))
            .ToDictionary(
                h => h.Key,
                h => h.Value
                    .Where(static v => v is not null)
                    .Select(static v => v!)
                    .ToArray());
    }
}