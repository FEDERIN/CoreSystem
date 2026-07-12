using Core.Memory.DependencyInjection;
using Core.Memory.Synchronization;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Memory.UnitTests.DependencyInjection;

public class MemoryRegistrationTests
{
    [Fact]
    public void AddCoreMemory_ShouldRegisterIAsyncKeyLock()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreMemory();

        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<IAsyncKeyLock>()
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void AddCoreMemory_ShouldRegisterMemoryLockProviderAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddCoreMemory();

        var provider = services.BuildServiceProvider();

        // Act
        var first = provider.GetRequiredService<IAsyncKeyLock>();
        var second = provider.GetRequiredService<IAsyncKeyLock>();

        // Assert
        first.Should().BeSameAs(second);
    }

    [Fact]
    public void AddCoreMemory_ShouldResolveMemoryLockProviderImplementation()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddCoreMemory();

        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetRequiredService<IAsyncKeyLock>();

        // Assert
        service.Should().BeOfType<MemoryLockProvider>();
    }
}