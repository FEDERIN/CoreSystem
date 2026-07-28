using Core.Idempotency.DependencyInjection;
using Core.Idempotency.Diagnostics;
using Core.Observability.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.UnitTests.DependencyInjection;

public class IdempotencyDiagnosticsRegistrationTests
{
    [Fact]
    public void AddCoreIdempotency_Should_Register_IdempotencyMetrics()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreIdempotency(options =>
        {
            options.Enabled = false;
        });

        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetRequiredService<IdempotencyMetrics>();
    }

    [Fact]
    public void AddCoreIdempotency_Should_Register_ObservabilityContributor()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreIdempotency(options =>
        {
            options.Enabled = false;
        });

        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetServices<IObservabilityContributor>()
                .Should()
                .ContainSingle(x => x is IdempotencyObservabilityContributor);
    }
}