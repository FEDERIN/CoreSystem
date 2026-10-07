using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Core.Observability.Abstractions.UnitTests;

/// <summary>
/// Contract tests for the extensibility interfaces. These guard the shapes that
/// third-party modules rely on: any implementation must be usable from DI and
/// must be discovered by the registries in CoreSystem.Observability.
/// </summary>
public sealed class ContributorContractTests
{
    private sealed class FakeObservabilityContributor : IObservabilityContributor
    {
        public IEnumerable<string> GetActivitySources() => ["Core.Fake"];

        public void ConfigureObservability(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new FakeMarker());
        }
    }

    private sealed class FakeHealthCheckContributor : IHealthCheckContributor
    {
        public void RegisterHealthChecks(IHealthChecksBuilder builder, IConfiguration configuration)
        {
            builder.AddCheck("fake", () => HealthCheckResult.Healthy());
        }
    }

    private sealed class FakeMarker;

    [Fact]
    public void ObservabilityContributor_RegistersServicesFromImplementation()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var contributor = new FakeObservabilityContributor();
        contributor.ConfigureObservability(services, configuration);

        var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<FakeMarker>());
    }

    [Fact]
    public void ObservabilityContributor_ExposesActivitySourceName()
    {
        var contributor = new FakeObservabilityContributor();

        Assert.Equal(["Core.Fake"], contributor.GetActivitySources());
    }

    [Fact]
    public void HealthCheckContributor_CanAddNamedHealthCheck()
    {
        var services = new ServiceCollection();
        var builder = services.AddHealthChecks();
        var configuration = new ConfigurationBuilder().Build();

        new FakeHealthCheckContributor().RegisterHealthChecks(builder, configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value;

        Assert.Equal(["fake"], options.Registrations.Select(r => r.Name));
    }
}