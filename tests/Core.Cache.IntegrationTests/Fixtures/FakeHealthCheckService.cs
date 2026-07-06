using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.Cache.IntegrationTests.Fixtures;

internal sealed class FakeHealthCheckService : HealthCheckService
{
    private readonly HealthReport _report;

    public FakeHealthCheckService(HealthStatus status)
    {
        _report = new HealthReport(
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
    }

    public override Task<HealthReport> CheckHealthAsync(
        Func<HealthCheckRegistration, bool>? predicate,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_report);
    }
}