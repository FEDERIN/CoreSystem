using System.Text;
using Core.Idempotency.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Idempotency.Fingerprint;

internal sealed class RequestFingerprintBuilder(
    IOptions<IdempotencyOptions> options)
    : IRequestFingerprintBuilder
{
    private readonly FingerprintOptions _options =
        options.Value.Fingerprint;

    public async ValueTask<string> BuildAsync(
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var request = context.Request;

        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        var builder = new StringBuilder();

        builder.AppendLine(request.Method);
        builder.AppendLine(request.Path.Value ?? string.Empty);

        if (_options.IncludeQueryString)
        {
            builder.AppendLine(
                request.QueryString.Value ?? string.Empty);
        }

        foreach (var headerName in _options.IncludedHeaders.Order())
        {
            if (request.Headers.TryGetValue(headerName, out var value))
            {
                builder.Append(headerName);
                builder.Append('=');
                builder.AppendLine(value.ToString());
            }
        }

        if (_options.IncludeContentType)
        {
            builder.AppendLine(
                request.ContentType ?? string.Empty);
        }

        request.Body.Position = 0;

        using var reader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync(cancellationToken);

        builder.Append(body);

        request.Body.Position = 0;

        return builder.ToString();
    }
}