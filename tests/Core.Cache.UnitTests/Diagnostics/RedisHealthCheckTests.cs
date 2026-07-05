using Core.Cache.Abstractions;
using Core.Cache.Diagnostics;
using Core.Cache.Pipeline.Behaviors;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StackExchange.Redis;

namespace Core.Cache.UnitTests.Diagnostics;

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

        var state = new Mock<IHealthState>();

        state.SetupGet(x => x.IsRedisHealthy)
             .Returns(true);

        var logger = NullLogger<RedisHealthCheck>.Instance;

        var healthCheck = new RedisHealthCheck(
            multiplexer.Object,
            state.Object,
            logger);

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