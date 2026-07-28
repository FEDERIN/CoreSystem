using Core.Idempotency.DependencyInjection;
using Core.Idempotency.Fingerprint;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.UnitTests.DependencyInjection;

public class IdempotencyFingerprintRegistrationTests
{
    [Fact]
    public void AddCoreIdempotency_Should_Register_FingerprintBuilder()
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
        provider.GetRequiredService<IRequestFingerprintBuilder>()
                .Should()
                .BeOfType<RequestFingerprintBuilder>();
    }

    [Fact]
    public void AddCoreIdempotency_Should_Register_RequestHasher()
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
        provider.GetRequiredService<IRequestHasher>()
                .Should()
                .BeOfType<Sha256RequestHasher>();
    }

    [Fact]
    public void AddCoreIdempotency_Should_Register_FingerprintProvider()
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
        provider.GetRequiredService<IRequestFingerprintProvider>()
                .Should()
                .BeOfType<DefaultRequestFingerprintProvider>();
    }
}