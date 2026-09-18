using Core.Observability.Abstractions;
using Core.RateLimiting.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.RateLimiting.UnitTests;

public sealed class RateLimitingObservabilityContributorTests
{
    [Fact]
    public void AddCoreRateLimiting_RegistersObservabilityContributor()
    {
        var services = new ServiceCollection();

        services.AddCoreRateLimiting(_ => { });

        using var provider = services.BuildServiceProvider();

        provider.GetService<IObservabilityContributor>()
            .Should()
            .NotBeNull();
    }
}
