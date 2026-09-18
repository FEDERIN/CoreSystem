using Core.RateLimiting.Abstractions;
using Core.RateLimiting.Options;
using Microsoft.AspNetCore.Http;

namespace Core.RateLimiting.Internal;

internal sealed class ClientPartitionKeyResolver(RateLimitingOptions options) : IRateLimitPartitionKeyResolver
{
    public string Resolve(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var subject = context.User.FindFirst(options.SubjectClaimType)?.Value;
        return !string.IsNullOrWhiteSpace(subject) ? $"subject:{subject}" : $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
    }
}
