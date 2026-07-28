using Core.Idempotency.Abstractions;
using Core.Idempotency.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.UnitTests.DependencyInjection;

public class IdempotencyApplicationBuilderExtensionsTests
{
    [Fact]
    public void UseCoreIdempotency_Should_Return_ApplicationBuilder()
    {
        // Arrange
        var services = new ServiceCollection();


        services.AddCoreIdempotency(options =>
        {
            options.Enabled = true;
            options.Provider = IdempotencyProviderType.PostgreSQL;
            options.PostgreSql.ConnectionString =
                "Host=localhost;Database=idempotency;Username=test;Password=test";
        });

        var provider = services.BuildServiceProvider();

        var app = new ApplicationBuilder(provider);

        // Act
        var result = app.UseCoreIdempotency();

        // Assert
        result.Should().BeSameAs(app);
    }

    [Fact]
    public void UseCoreIdempotency_Should_Throw_When_ApplicationBuilder_Is_Null()
    {
        // Arrange
        IApplicationBuilder app = null!;

        // Act
        var action = () => app.UseCoreIdempotency();

        // Assert
        action.Should()
              .Throw<ArgumentNullException>()
              .WithParameterName("app");
    }

    [Fact]
    public void UseCoreIdempotency_Should_Throw_When_CoreIdempotency_Is_Not_Registered()
    {
        // Arrange
        var services = new ServiceCollection();

        var provider = services.BuildServiceProvider();

        var app = new ApplicationBuilder(provider);

        // Act
        var action = () => app.UseCoreIdempotency();

        // Assert
        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage(IdempotencyMessages.MissingRegistration);
    }
}