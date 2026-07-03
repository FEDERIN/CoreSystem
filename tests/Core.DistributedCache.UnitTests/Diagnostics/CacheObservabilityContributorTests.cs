using Core.DistributedCache.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;

namespace Core.DistributedCache.UnitTests.Diagnostics;

public sealed class CacheObservabilityContributorTests
{
    [Fact]
    public void GetActivitySources_ShouldReturnDistributedCacheSource()
    {
        // Arrange
        var contributor = new CacheObservabilityContributor();

        // Act
        var sources = contributor.GetActivitySources();

        // Assert
        sources.Should().ContainSingle();
        sources.Should().Contain("Core.DistributedCache");
    }

    [Fact]
    public void ConfigureObservability_ShouldRegisterOpenTelemetry()
    {
        // Arrange
        var services = new ServiceCollection();

        var contributor = new CacheObservabilityContributor();

        // Act
        contributor.ConfigureObservability(
            services,
            new ConfigurationBuilder().Build());

        var provider = services.BuildServiceProvider();

        // Assert

        provider.GetService<MeterProvider>()
            .Should()
            .NotBeNull();
    }
}