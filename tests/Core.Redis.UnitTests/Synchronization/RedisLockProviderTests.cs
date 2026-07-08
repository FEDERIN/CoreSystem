using Core.Redis.Options;
using Core.Redis.Synchronization;
using Moq;
using StackExchange.Redis;

namespace Core.Redis.UnitTests.Synchronization;

public sealed class RedisLockProviderTests
{
    [Fact]
    public async Task AcquireAsync_ShouldAcquireLock_WhenAvailable()
    {
        // Arrange
        var database = new Mock<IDatabase>();

        database
            .Setup(x => x.LockTakeAsync(
                "order:1",
                It.IsAny<RedisValue>(),
                TimeSpan.FromSeconds(30),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var multiplexer = new Mock<IConnectionMultiplexer>();

        multiplexer
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(database.Object);

        var options = new RedisLockOptions();

        var provider = new RedisLockProvider(
            multiplexer.Object,
            options);

        // Act
        var handle = await provider.AcquireAsync("order:1", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(handle);

        database.Verify(x =>
            x.LockTakeAsync(
                "order:1",
                It.IsAny<RedisValue>(),
                options.LockDuration,
                It.IsAny<CommandFlags>()),
            Times.Once);
    }


    [Fact]
    public async Task AcquireAsync_ShouldReleaseLock_WhenDisposed()
    {
        // Arrange
        var database = new Mock<IDatabase>();

        database
            .Setup(x => x.LockTakeAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var multiplexer = new Mock<IConnectionMultiplexer>();

        multiplexer
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(database.Object);

        var provider = new RedisLockProvider(
            multiplexer.Object,
            new RedisLockOptions());

        // Act
        var handle = await provider.AcquireAsync("customer", TestContext.Current.CancellationToken);

        handle.Dispose();

        // Assert
        database.Verify(x =>
            x.LockRelease(
                "customer",
                It.IsAny<RedisValue>(),
                It.IsAny<CommandFlags>()),
            Times.Once);
    }

    [Fact]
    public async Task AcquireAsync_ShouldThrowOperationCanceledException_WhenCancellationRequested()
    {
        // Arrange
        var database = new Mock<IDatabase>();

        var multiplexer = new Mock<IConnectionMultiplexer>();

        multiplexer
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(database.Object);

        var provider = new RedisLockProvider(
            multiplexer.Object,
            new RedisLockOptions());

        using var cts = new CancellationTokenSource();

        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            provider.AcquireAsync("order", cts.Token));
    }

    [Fact]
    public async Task AcquireAsync_ShouldRetryUntilLockIsAvailable()
    {
        // Arrange
        var database = new Mock<IDatabase>();

        database
            .SetupSequence(x => x.LockTakeAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        var multiplexer = new Mock<IConnectionMultiplexer>();

        multiplexer
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(database.Object);

        var provider = new RedisLockProvider(
            multiplexer.Object,
            new RedisLockOptions
            {
                RetryDelay = TimeSpan.Zero
            });

        // Act
        var handle = await provider.AcquireAsync("resource", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(handle);

        database.Verify(x =>
            x.LockTakeAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CommandFlags>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task AcquireAsync_ShouldThrowTimeoutException_WhenMaxWaitTimeIsExceeded()
    {
        // Arrange
        var database = new Mock<IDatabase>();

        database
            .Setup(x => x.LockTakeAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        var multiplexer = new Mock<IConnectionMultiplexer>();

        multiplexer
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(database.Object);

        var provider = new RedisLockProvider(
            multiplexer.Object,
            new RedisLockOptions
            {
                RetryDelay = TimeSpan.Zero,
                MaxWaitTime = TimeSpan.FromMilliseconds(10)
            });

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(() =>
            provider.AcquireAsync("resource", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Dispose_ShouldPropagateException_WhenLockReleaseFails()
    {
        // Arrange
        var database = new Mock<IDatabase>();

        database
            .Setup(x => x.LockTakeAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        database
            .Setup(x => x.LockRelease(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<CommandFlags>()))
            .Throws(new RedisConnectionException(
                ConnectionFailureType.SocketFailure,
                "Redis unavailable"));

        var multiplexer = new Mock<IConnectionMultiplexer>();

        multiplexer
            .Setup(x => x.GetDatabase(
                It.IsAny<int>(),
                It.IsAny<object>()))
            .Returns(database.Object);

        var provider = new RedisLockProvider(
            multiplexer.Object,
            new RedisLockOptions());

        var handle = await provider.AcquireAsync(
            "customer",
            TestContext.Current.CancellationToken);

        // Act & Assert
        Assert.Throws<RedisConnectionException>(() =>
            handle.Dispose());
    }
}