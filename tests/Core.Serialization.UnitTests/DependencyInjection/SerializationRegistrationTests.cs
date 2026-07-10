using Core.Serialization.Abstractions;
using Core.Serialization.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Serialization.UnitTests.DependencyInjection;

public class SerializationRegistrationTests
{
    [Fact]
    public void AddCoreSerialization_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddCoreSerialization();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddCoreSerialization_ShouldRegisterPayloadSerializer()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreSerialization();

        // Assert
        using var provider = services.BuildServiceProvider();

        var serializer = provider.GetService<IPayloadSerializer>();

        Assert.NotNull(serializer);
    }

    [Fact]
    public void AddCoreSerialization_ShouldResolvePayloadSerializer()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreSerialization();

        // Assert
        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IPayloadSerializer>());
    }

    [Fact]
    public void AddCoreSerialization_ShouldBeResolvableMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddCoreSerialization();

        using var provider = services.BuildServiceProvider();

        // Act
        var first = provider.GetRequiredService<IPayloadSerializer>();
        var second = provider.GetRequiredService<IPayloadSerializer>();

        // Assert
        Assert.NotNull(first);
        Assert.NotNull(second);
    }
}