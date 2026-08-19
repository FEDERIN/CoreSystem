using Core.Cache.Redis.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.Cache.Redis.UnitTests.Diagnostics;

public sealed class RedisHealthContributorTests
{
    [Fact]
    public void RegisterHealthChecks_RegistersRedisHealthCheck()
    {
        var services = new ServiceCollection();

        var builder = services
            .AddHealthChecks();

        var contributor =
            new RedisHealthContributor();

        contributor.RegisterHealthChecks(
            builder,
            new ConfigurationBuilder().Build());

        var descriptor =
            services.FirstOrDefault(
                x => x.ServiceType == typeof(HealthCheckService));

        Assert.NotNull(descriptor);
    }
}