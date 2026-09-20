using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Observability.Correlation;

/// <summary>Establishes a safe correlation identifier for the current request.</summary>
public sealed class CorrelationIdMiddleware(
    RequestDelegate next,
    IOptions<CorrelationOptions> options,
    ILogger<CorrelationIdMiddleware> logger)
{
    private readonly CorrelationOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.Enabled)
        {
            await next(context);
            return;
        }

        string correlationId = GetCorrelationId(context.Request.Headers[_options.HeaderName]);
        context.Items[CoreCorrelationConstants.HttpContextItemKey] = correlationId;

        if (_options.IncludeInResponse)
        {
            context.Response.Headers[_options.HeaderName] = correlationId;
        }

        using (logger.BeginScope(new Dictionary<string, object>
        {
            [CoreCorrelationConstants.LogPropertyName] = correlationId
        }))
        {
            await next(context);
        }
    }

    private string GetCorrelationId(Microsoft.Extensions.Primitives.StringValues values)
    {
        string? candidate = values.Count == 1 ? values[0] : null;
        return IsValid(candidate) ? candidate! : Guid.NewGuid().ToString("D");
    }

    private bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > _options.MaximumLength)
        {
            return false;
        }

        return value.All(character => char.IsAsciiLetterOrDigit(character) || character is '-' or '_' or '.');
    }
}
