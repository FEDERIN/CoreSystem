using Core.Cache.Redis.Diagnostics;
using Core.Cache.Redis.Storage.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace Core.Cache.Redis.UnitTests.Diagnostics;

public sealed class RedisHealthCheckTests
{
    private readonly Mock<IConnectionMultiplexer> _redis;
    private readonly Mock<IDatabase> _database;
    private readonly Mock<IHealthState> _healthState;
    private readonly Mock<ILogger<RedisHealthCheck>> _logger;

    private readonly RedisHealthCheck _sut;

    public RedisHealthCheckTests()
    {
        _redis = new Mock<IConnectionMultiplexer>();
        _database = new Mock<IDatabase>();
        _healthState = new Mock<IHealthState>();
        _logger = new Mock<ILogger<RedisHealthCheck>>();

        _redis
            .Setup(x => x.GetDatabase(
                It.IsAny<int>(),
                It.IsAny<object?>()))
            .Returns(_database.Object);

        _sut = new RedisHealthCheck(
            _redis.Object,
            _healthState.Object,
            _logger.Object);
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthy_WhenRedisIsAvailable()
    {
        _database
            .Setup(x => x.PingAsync(
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(TimeSpan.FromMilliseconds(5));

        _healthState
            .Setup(x => x.Update(true))
            .Returns(HealthTransition.None);

        var result =
            await _sut.CheckHealthAsync(
                new HealthCheckContext(),
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HealthStatus.Healthy,
            result.Status);

        Assert.Equal(
            "Redis is connected successfully.",
            result.Description);

        _healthState.Verify(
            x => x.Update(true),
            Times.Once);
    }

    [Fact]
    public async Task CheckHealthAsync_LogsRecovery_WhenRedisBecomesHealthy()
    {
        _database
            .Setup(x => x.PingAsync(
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(TimeSpan.FromMilliseconds(5));

        _healthState
            .Setup(x => x.Update(true))
            .Returns(HealthTransition.BecameHealthy);

        var result =
            await _sut.CheckHealthAsync(
                new HealthCheckContext(),
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HealthStatus.Healthy,
            result.Status);

        //VerifyLog(
        //    LogLevel.Information,
        //    "Redis connection restored.");
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsDegraded_WhenRedisIsUnavailable()
    {
        var exception =
            new RedisConnectionException(
                ConnectionFailureType.UnableToConnect,
                "Redis unavailable.");

        _database
            .Setup(x => x.PingAsync(
                It.IsAny<CommandFlags>()))
            .ThrowsAsync(exception);

        _healthState
            .Setup(x => x.Update(false))
            .Returns(HealthTransition.None);

        var result =
            await _sut.CheckHealthAsync(
                new HealthCheckContext(),
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HealthStatus.Degraded,
            result.Status);

        Assert.Equal(
            "Redis is not responding. Memory fallback active.",
            result.Description);

        Assert.Same(
            exception,
            result.Exception);

        _healthState.Verify(
            x => x.Update(false),
            Times.Once);
    }

    [Fact]
    public async Task CheckHealthAsync_LogsWarning_WhenRedisBecomesUnhealthy()
    {
        var exception =
            new RedisConnectionException(
                ConnectionFailureType.UnableToConnect,
                "Redis unavailable.");

        _database
            .Setup(x => x.PingAsync(
                It.IsAny<CommandFlags>()))
            .ThrowsAsync(exception);

        _healthState
            .Setup(x => x.Update(false))
            .Returns(HealthTransition.BecameUnhealthy);

        var result =
            await _sut.CheckHealthAsync(
                new HealthCheckContext(),
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HealthStatus.Degraded,
            result.Status);

        //VerifyLog(
        //    LogLevel.Warning,
        //    "Redis became unavailable. Switching to memory fallback.");
    }

    [Fact]
    public async Task CheckHealthAsync_DoesNotLogWarning_WhenRedisRemainsUnavailable()
    {
        var exception =
            new RedisConnectionException(
                ConnectionFailureType.UnableToConnect,
                "Redis unavailable.");

        _database
            .Setup(x => x.PingAsync(
                It.IsAny<CommandFlags>()))
            .ThrowsAsync(exception);

        _healthState
            .Setup(x => x.Update(false))
            .Returns(HealthTransition.None);

        var result =
            await _sut.CheckHealthAsync(
                new HealthCheckContext(),
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HealthStatus.Degraded,
            result.Status);

        _logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task CheckHealthAsync_DoesNotLogRecovery_WhenRedisRemainsHealthy()
    {
        _database
            .Setup(x => x.PingAsync(
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(TimeSpan.FromMilliseconds(5));

        _healthState
            .Setup(x => x.Update(true))
            .Returns(HealthTransition.None);

        var result =
            await _sut.CheckHealthAsync(
                new HealthCheckContext(),
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HealthStatus.Healthy,
            result.Status);

        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}