using Core.Resilience.Internal.Helpers;

namespace Core.Resilience.UnitTests.Internal.Helpers;

public sealed class ExceptionMatcherTests
{
    [Fact]
    public void Matches_ShouldReturnFalse_WhenExceptionDoesNotMatch()
    {
        // Arrange
        var exception = new InvalidOperationException();

        var exceptionTypes = new[]
        {
            typeof(TimeoutException)
        };

        // Act
        var result = ExceptionMatcher.Matches(
            exception,
            exceptionTypes);

        // Assert
        Assert.False(result);
    }
}