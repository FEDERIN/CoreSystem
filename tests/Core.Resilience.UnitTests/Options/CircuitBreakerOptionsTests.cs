using Core.Resilience.Options;
using FluentAssertions;

namespace Core.Resilience.UnitTests.Options;

public sealed class CircuitBreakerOptionsTests
{
    [Fact]
    public void Handle_ShouldAddHandledExceptions()
    {
        // Arrange
        var options = new CircuitBreakerOptions();

        // Act
        var result = options.Handle(
            typeof(TimeoutException),
            typeof(InvalidOperationException));

        // Assert
        result.Should().BeSameAs(options);

        options.HandledExceptions.Should().Contain(
            typeof(TimeoutException));

        options.HandledExceptions.Should().Contain(
            typeof(InvalidOperationException));
    }

    [Fact]
    public void Handle_ShouldThrowArgumentException_WhenTypeDoesNotDeriveFromException()
    {
        // Arrange
        var options = new CircuitBreakerOptions();

        // Act
        var action = () => options.Handle(typeof(string));

        // Assert
        action.Should()
            .Throw<ArgumentException>()
            .WithParameterName("exceptionType")
            .WithMessage("*must derive from Exception*");
    }
}