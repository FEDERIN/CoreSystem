using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using StackExchange.Redis;

namespace Core.DistributedCache.UnitTests.Diagnostics;

public sealed class RedisHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenPingThrows_ShouldReturnDegraded()
    {
        // Arrange
        var database = new Mock<IDatabase>();

        database
            .Setup(x => x.PingAsync(It.IsAny<CommandFlags>()))
            .ThrowsAsync(new RedisConnectionException(
                ConnectionFailureType.UnableToConnect,
                "Redis unavailable"));

        var multiplexer = new Mock<IConnectionMultiplexer>();

        multiplexer
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(database.Object);

        var state = new Mock<IRedisHealthState>();

        state.SetupGet(x => x.IsRedisHealthy)
             .Returns(true);

        var healthCheck = new RedisHealthCheck(
            multiplexer.Object,
            state.Object);

        // Act
        var result = await healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            TestContext.Current.CancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Degraded);

        result.Description.Should().Be(
            "Redis is not responding. Memory fallback active.");

        result.Exception.Should().BeOfType<RedisConnectionException>();
    }
}