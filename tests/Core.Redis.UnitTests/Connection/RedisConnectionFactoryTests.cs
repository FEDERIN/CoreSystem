using Core.Redis.Connection;

namespace Core.Redis.UnitTests.Connection;

public sealed class RedisConnectionFactoryTests
{
    private readonly RedisConnectionFactory _factory = new();

    [Fact]
    public void Create_ShouldThrow_WhenNoEndpointsConfigured()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _factory.Create(null));

        Assert.Equal(
            "At least one Redis endpoint must be configured.",
            exception.Message);
    }

    [Fact]
    public void Create_ShouldInvokeConfigureDelegate()
    {
        // Arrange
        var invoked = false;

        // Act
        Assert.ThrowsAny<Exception>(() =>
            _factory.Create(options =>
            {
                invoked = true;
            }));

        // Assert
        Assert.True(invoked);
    }

    [Fact]
    public void Create_ShouldAcceptEndpointConfiguration()
    {
        // Act
        var multiplexer = _factory.Create(options =>
        {
            options.EndPoints.Add("localhost:6379");
        });

        // Assert
        Assert.NotNull(multiplexer);

        multiplexer.Dispose();
    }
}