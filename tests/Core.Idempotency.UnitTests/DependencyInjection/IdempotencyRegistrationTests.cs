using Core.Idempotency.Abstractions;
using Core.Idempotency.DependencyInjection;
using Core.Idempotency.Options;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.UnitTests.DependencyInjection;

public class IdempotencyRegistrationTests
{
    [Fact]
    public void AddCoreIdempotency_Should_Register_Required_Services()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreIdempotency(options =>
        {
            options.Enabled = true;
            options.Provider = IdempotencyProviderType.PostgreSQL;
            options.PostgreSql.ConnectionString =
                "Host=localhost;Database=idempotency;Username=test;Password=test";
        });

        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetRequiredService<IdempotencyOptions>();
        provider.GetRequiredService<IIdempotencyService>();
        provider.GetRequiredService<IIdempotencyKeyResolver>();
        provider.GetRequiredService<IIdempotencyStorage>();
    }

    [Fact]
    public void AddCoreIdempotency_Should_Register_Configured_Options()
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
        var options = provider.GetRequiredService<IdempotencyOptions>();

        options.Enabled.Should().BeFalse();
    }

    [Fact]
    public void AddCoreIdempotency_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddCoreIdempotency(options => options.Enabled = false);

        // Assert
        result.Should().BeSameAs(services);
    }


    [Fact]
    public void AddCoreIdempotency_Should_Register_OtherProvider_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var action = () => services.AddCoreIdempotency(options =>
        {
            options.Enabled = true;
            options.Provider = (IdempotencyProviderType)999;
        });

        // Assert
        action.Should()
              .Throw<NotSupportedException>()
              .WithMessage(IdempotencyMessages.UnsupportedProvider((IdempotencyProviderType)999));
    }
}