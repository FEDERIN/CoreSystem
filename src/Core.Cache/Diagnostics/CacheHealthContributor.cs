using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.Cache.Diagnostics;

internal sealed class CacheHealthContributor : IHealthCheckContributor
{
    public void RegisterHealthChecks(IHealthChecksBuilder builder, IConfiguration configuration)
    {
        builder.AddCheck<RedisHealthCheck>("redis_cache",
            HealthStatus.Degraded,
            tags: ["cache"]);
    }
}