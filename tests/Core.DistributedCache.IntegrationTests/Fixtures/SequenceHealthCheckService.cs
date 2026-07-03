using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.DistributedCache.IntegrationTests.Fixtures;

internal sealed class SequenceHealthCheckService(params HealthStatus[] statuses) : HealthCheckService
{
    private readonly Queue<HealthStatus> _statuses = new(statuses);

    public override Task<HealthReport> CheckHealthAsync(
        Func<HealthCheckRegistration, bool>? predicate,
        CancellationToken cancellationToken = default)
    {
        var status = _statuses.Dequeue();

        var report = new HealthReport(
            new Dictionary<string, HealthReportEntry>
            {
                ["redis_cache"] = new HealthReportEntry(
                    status,
                    "Redis",
                    TimeSpan.Zero,
                    exception: null,
                    data: null)
            },
            TimeSpan.Zero);

        return Task.FromResult(report);
    }
}