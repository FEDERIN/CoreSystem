using Core.Idempotency.DependencyInjection;
using Core.Idempotency.ExceptionHandling;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.UnitTests.DependencyInjection;

public class IdempotencyExceptionRegistrationTests
{
    [Fact]
    public void AddCoreIdempotency_Should_Register_IdempotencyExceptionHandler()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddProblemDetails();

        services.AddCoreIdempotency(options =>
        {
            options.Enabled = false;
        });

        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetServices<IExceptionHandler>()
                .Should()
                .ContainSingle(handler => handler is IdempotencyExceptionHandler);
    }
}