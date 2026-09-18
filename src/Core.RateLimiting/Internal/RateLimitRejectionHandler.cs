using Core.RateLimiting.Diagnostics;
using Core.RateLimiting.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.RateLimiting;

namespace Core.RateLimiting.Internal;

internal sealed class RateLimitRejectionHandler(
    RateLimitingOptions options,
    RateLimitingMetrics metrics,
    ILogger<RateLimitRejectionHandler> logger)
{
    private const string TooManyRequestsType = "https://httpstatuses.com/429";

    private readonly RateLimitingOptions _options = options;
    private readonly RateLimitingMetrics _metrics = metrics;
    private readonly ILogger<RateLimitRejectionHandler> _logger = logger;

    public async ValueTask HandleAsync(OnRejectedContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        var httpContext = context.HttpContext;
        var retryAfterSeconds = GetRetryAfterSeconds(context.Lease);

        AddRetryAfterHeader(httpContext, retryAfterSeconds);
        RecordRejection(retryAfterSeconds);

        var problemDetails = CreateProblemDetails(retryAfterSeconds);
        var problemDetailsService =
            httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static int? GetRetryAfterSeconds(RateLimitLease lease)
    {
        if (!lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            return null;
        }

        return Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
    }

    private static void AddRetryAfterHeader(
        HttpContext httpContext,
        int? retryAfterSeconds)
    {
        if (retryAfterSeconds is not null)
        {
            httpContext.Response.Headers.RetryAfter = retryAfterSeconds.Value.ToString();
        }
    }

    private void RecordRejection(int? retryAfterSeconds)
    {
        _metrics.RecordRejection(_options.PolicyName);

        _logger.LogWarning(
            "Rate limit rejected a request for policy {RateLimitPolicy}. " +
            "RetryAfterSeconds: {RetryAfterSeconds}",
            _options.PolicyName,
            retryAfterSeconds);
    }

    private ProblemDetails CreateProblemDetails(int? retryAfterSeconds)
    {
        var problemDetails = new ProblemDetails
        {
            Type = TooManyRequestsType,
            Title = "Too Many Requests",
            Status = StatusCodes.Status429TooManyRequests,
            Detail = "The request rate limit has been exceeded."
        };

        problemDetails.Extensions["policy"] = _options.PolicyName;

        if (retryAfterSeconds is not null)
        {
            problemDetails.Extensions["retryAfterSeconds"] = retryAfterSeconds.Value;
        }

        return problemDetails;
    }
}
