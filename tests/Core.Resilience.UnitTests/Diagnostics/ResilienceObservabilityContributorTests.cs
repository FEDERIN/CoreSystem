using Core.Resilience.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;

namespace Core.Resilience.UnitTests.Diagnostics;

public sealed class ResilienceObservabilityContributorTests
{
    [Fact]
    public void GetActivitySources_ShouldReturnResilienceSource()
    {
        // Arrange
        var contributor = new ResilienceObservabilityContributor();

        // Act
        var sources = contributor.GetActivitySources();

        // Assert
        sources.Should().ContainSingle();
        sources.Should().Contain("Core.Resilience");
    }

    [Fact]
    public void ConfigureObservability_ShouldRegisterOpenTelemetry()
    {
        // Arrange
        var services = new ServiceCollection();

        var contributor = new ResilienceObservabilityContributor();

        // Act
        contributor.ConfigureObservability(
            services,
            new ConfigurationBuilder().Build());

        using var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<MeterProvider>()
            .Should()
            .NotBeNull();
    }
}