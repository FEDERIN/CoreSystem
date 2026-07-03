using FluentAssertions;

namespace Core.Cache.UnitTests.Behaviors;

public class ExistsAsyncBehaviorTests : MemoryCacheTestBase
{
    [Fact]
    public async Task ExistsAsync_WhenKeyExists_ReturnsTrue()
    {
        // Arrange
        var cache = CreateCache();

        const string key = "users:2";

        await cache.SetAsync(key, "Juan", TimeSpan.FromMinutes(5), ["user"], TestContext.Current.CancellationToken);

        // Act
        var exists = await cache.ExistsAsync(key, TestContext.Current.CancellationToken);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenKeyDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var cache = CreateCache();

        // Act
        var exists = await cache.ExistsAsync("users:999", TestContext.Current.CancellationToken);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_AfterRemove_ShouldReturnFalse()
    {
        // Arrange
        var cache = CreateCache();

        await cache.SetAsync(
            "customer:1",
            new CustomerDto(),
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        await cache.RemoveAsync(
            "customer:1",
            TestContext.Current.CancellationToken);

        var exists = await cache.ExistsAsync(
            "customer:1",
            TestContext.Current.CancellationToken);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_AfterExpiration_ShouldReturnFalse()
    {
        var cache = CreateCache();

        await cache.SetAsync(
            "customer:1",
            new CustomerDto(),
            TimeSpan.FromMilliseconds(100),
            ["customers"],
            TestContext.Current.CancellationToken);

        await Task.Delay(300, TestContext.Current.CancellationToken);

        var exists = await cache.ExistsAsync(
            "customer:1",
            TestContext.Current.CancellationToken);

        exists.Should().BeFalse();
    }
}