using Core.Memory.Synchronization;
using FluentAssertions;
using Moq;

namespace Core.Memory.UnitTests.Synchronization;

public sealed class MemoryLockProviderTests
{
    [Fact]
    public async Task AcquireAsync_WhenKeyIsAvailable_ShouldReturnDisposable()
    {
        var provider = new MemoryLockProvider();

        var handle = await provider.AcquireAsync("order", TestContext.Current.CancellationToken);

        handle.Should().NotBeNull();

        handle.Dispose();
    }

    [Fact]
    public async Task AcquireAsync_WhenLockIsAvailable_ShouldReturnMemoryLockReleaser()
    {
        // Arrange
        var registry = new Mock<ILockRegistry>();

        var entry = new MemoryLockEntry();

        registry
            .Setup(r => r.GetOrCreate("key"))
            .Returns(entry);

        var provider = new MemoryLockProvider(registry.Object);

        // Act
        var releaser = await provider.AcquireAsync("key", TestContext.Current.CancellationToken);

        // Assert
        releaser.Should().NotBeNull();
        releaser.Should().BeOfType<MemoryLockReleaser>();
    }

    [Fact]
    public async Task AcquireAsync_WhenWaitIsCancelled_ShouldReleaseRegistry()
    {
        // Arrange
        var registry = new Mock<ILockRegistry>();

        var entry = new MemoryLockEntry();

        registry
            .Setup(x => x.GetOrCreate("key"))
            .Returns(entry);

        var provider = new MemoryLockProvider(registry.Object);

        var first = await provider.AcquireAsync("key", TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();

        var task = provider.AcquireAsync("key", cts.Token);

        await Task.Delay(50, TestContext.Current.CancellationToken);

        cts.Cancel();

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => task);

        registry.Verify(
            x => x.Release("key", entry),
            Times.Once);

        first.Dispose();
    }

}
