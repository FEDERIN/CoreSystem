using Core.DistributedCache.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Core.DistributedCache.UnitTests.Diagnostics;

public sealed class CacheHealthContributorTests
{
    [Fact]
    public void RegisterHealthChecks_ShouldRegisterRedisHealthCheck()
    {
        // Arrange
        var services = new ServiceCollection();

        var builder = services.AddHealthChecks();

        var contributor = new CacheHealthContributor();

        var configuration = new ConfigurationBuilder().Build();

        // Act
        contributor.RegisterHealthChecks(builder, configuration);

        // Assert
        var descriptor = services
            .SingleOrDefault(s =>
                s.ServiceType == typeof(HealthCheckService));

        descriptor.Should().NotBeNull();

        var registrations = services
            .Where(s => s.ServiceType == typeof(IConfigureOptions<HealthCheckServiceOptions>));

        registrations.Should().NotBeEmpty();
    }
}