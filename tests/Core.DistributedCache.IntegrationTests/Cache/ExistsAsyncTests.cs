using Core.Cache.IntegrationTests.Fixtures;
using FluentAssertions;

namespace Core.Cache.IntegrationTests.Cache;


[Collection("Redis")]
public sealed class ExistsAsyncTests(RedisContainerFixture fixture)
    : RedisCacheTestBase(fixture),
      IClassFixture<RedisContainerFixture>
{
    [Fact]
    public async Task ExistsAsync_WhenKeyExists_ShouldReturnTrue()
    {
        // Arrange
        await Cache.SetAsync(
            "customer:1",
            new CustomerDto
            {
                Id = 1,
                Name = "Juan"
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        // Act
        var exists = await Cache.ExistsAsync("customer:1", TestContext.Current.CancellationToken);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenKeyDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var exists = await Cache.ExistsAsync("customer:999", TestContext.Current.CancellationToken);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_AfterRemove_ShouldReturnFalse()
    {
        // Arrange
        await Cache.SetAsync(
            "customer:1",
            new CustomerDto
            {
                Id = 1,
                Name = "Juan"
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        await Cache.RemoveAsync("customer:1", TestContext.Current.CancellationToken);

        // Act
        var exists = await Cache.ExistsAsync("customer:1", TestContext.Current.CancellationToken);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_AfterExpiration_ShouldReturnFalse()
    {
        // Arrange
        await Cache.SetAsync(
            "customer:1",
            new CustomerDto
            {
                Id = 1,
                Name = "Juan"
            },
            TimeSpan.FromMilliseconds(300),
            ["customers"],
            TestContext.Current.CancellationToken);

        await Task.Delay(600, TestContext.Current.CancellationToken);

        // Act
        var exists = await Cache.ExistsAsync("customer:1", TestContext.Current.CancellationToken);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_AfterTagInvalidation_ShouldReturnFalse()
    {
        // Arrange
        await Cache.SetAsync(
            "customer:1",
            new CustomerDto
            {
                Id = 1,
                Name = "Juan"
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        // Act
        await Cache.InvalidateByTagAsync("customers", TestContext.Current.CancellationToken);

        // Assert
        var exists = await Cache.ExistsAsync("customer:1", TestContext.Current.CancellationToken);

        exists.Should().BeFalse();
    }
}