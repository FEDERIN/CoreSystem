using System.Diagnostics.Metrics;

namespace Core.RateLimiting.Diagnostics;

public sealed class RateLimitingMetrics(IMeterFactory meterFactory)
{
    public const string MeterName = "Core.RateLimiting";
    private readonly Counter<long> _rejections = meterFactory.Create(MeterName, "1.0.0")
        .CreateCounter<long>("rate_limiting.rejected_requests", "{requests}", "Total requests rejected by the rate limiter.");

    public void RecordRejection(string policyName) => _rejections.Add(1, new KeyValuePair<string, object?>("rate_limit.policy", policyName));
}
