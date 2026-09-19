using System.Diagnostics.Metrics;

namespace Core.RateLimiting.Diagnostics;

/// <summary>
/// Provides metrics for monitoring rate-limiting activity.
/// </summary>
/// <param name="meterFactory">
/// The factory used to create the meter that owns the rate-limiting metrics.
/// </param>
public sealed class RateLimitingMetrics(IMeterFactory meterFactory)
{
    /// <summary>
    /// Gets the name of the meter used by Core.RateLimiting.
    /// </summary>
    public const string MeterName = "Core.RateLimiting";

    private readonly Counter<long> _rejections = meterFactory
        .Create(MeterName, "1.0.0")
        .CreateCounter<long>(
            "rate_limiting.rejected_requests",
            "{requests}",
            "Total requests rejected by the rate limiter.");

    /// <summary>
    /// Records a request rejected by the rate limiter.
    /// </summary>
    /// <param name="policyName">
    /// The name of the rate-limiting policy responsible for rejecting the request.
    /// </param>
    public void RecordRejection(string policyName) =>
        _rejections.Add(
            1,
            new KeyValuePair<string, object?>(
                "rate_limit.policy",
                policyName));
}