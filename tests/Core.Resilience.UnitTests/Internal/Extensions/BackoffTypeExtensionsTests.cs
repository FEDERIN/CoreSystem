using Core.Resilience.Internal.Extensions;
using Core.Resilience.Options;
using Polly;

namespace Core.Resilience.UnitTests.Internal.Extensions;

public sealed class BackoffTypeExtensionsTests
{
    [Theory]
    [InlineData(BackoffType.Constant, DelayBackoffType.Constant)]
    [InlineData(BackoffType.Linear, DelayBackoffType.Linear)]
    [InlineData(BackoffType.Exponential, DelayBackoffType.Exponential)]
    public void ToPolly_ShouldReturnExpectedDelayBackoffType(
        BackoffType backoffType,
        DelayBackoffType expected)
    {
        // Act
        var result = backoffType.ToPolly();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToPolly_ShouldThrowArgumentOutOfRangeException_WhenBackoffTypeIsInvalid()
    {
        // Arrange
        var backoffType = (BackoffType)999;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            backoffType.ToPolly());
    }
}