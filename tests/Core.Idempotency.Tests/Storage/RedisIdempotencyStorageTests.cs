using Core.Idempotency.Models;
using Core.Idempotency.Storage.Redis;
using FluentAssertions;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace Core.Idempotency.Tests.Storage;

public class RedisIdempotencyStorageTests
{
    [Fact]
    public async Task GetAsync_WhenKeyExists_ReturnsIdempotencyResponse()
    {
        // Arrange
        var mockRedis = new Mock<IConnectionMultiplexer>();
        var mockDb = new Mock<IDatabase>();
        var expectedResponse = new IdempotencyResponse
        {
            StatusCode = 200,
            ContentType = "application/json",
            Body = "{\"result\":\"success\"}"
        };

        var serialized = System.Text.Json.JsonSerializer.Serialize(expectedResponse);
        mockDb
            .Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisValue)serialized);

        mockRedis
            .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object?>()))
            .Returns(mockDb.Object);

        var storage = new RedisIdempotencyStorage(mockRedis.Object);

        // Act
        var result = await storage.GetAsync("test-key");

        // Assert
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);
        result.ContentType.Should().Be("application/json");
        result.Body.Should().Contain("success");
    }

    [Fact]
    public async Task GetAsync_WhenKeyDoesNotExist_ReturnsNull()
    {
        // Arrange
        var mockRedis = new Mock<IConnectionMultiplexer>();
        var mockDb = new Mock<IDatabase>();
        
        mockDb
            .Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        mockRedis
            .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object?>()))
            .Returns(mockDb.Object);

        var storage = new RedisIdempotencyStorage(mockRedis.Object);

        // Act
        var result = await storage.GetAsync("nonexistent-key");

        // Assert
        result.Should().BeNull();
    }
}