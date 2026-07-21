using Core.Resilience.Internal.Helpers;
using Polly;

namespace Core.Resilience.UnitTests.Internal.Helpers;

public sealed class PredicateBuilderFactoryTests
{
    [Fact]
    public void Create_ShouldThrowArgumentNullException_WhenExceptionTypesIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PredicateBuilderFactory.Create(null!));
    }

    [Fact]
    public void Create_ShouldReturnPredicateBuilder_WhenCollectionIsEmpty()
    {
        // Arrange
        var exceptionTypes = Array.Empty<Type>();

        // Act
        var result = PredicateBuilderFactory.Create(exceptionTypes);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PredicateBuilder>(result);
    }

    [Fact]
    public void Create_ShouldReturnPredicateBuilder_WhenExceptionTypesAreValid()
    {
        // Arrange
        var exceptionTypes = new[]
        {
            typeof(TimeoutException),
            typeof(InvalidOperationException)
        };

        // Act
        var result = PredicateBuilderFactory.Create(exceptionTypes);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PredicateBuilder>(result);
    }

    [Fact]
    public void Create_ShouldThrowInvalidOperationException_WhenTypeDoesNotDeriveFromException()
    {
        // Arrange
        var exceptionTypes = new[]
        {
            typeof(string)
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            PredicateBuilderFactory.Create(exceptionTypes));

        // Assert
        Assert.Equal(
            $"Type '{typeof(string).FullName}' must derive from Exception.",
            exception.Message);
    }
}