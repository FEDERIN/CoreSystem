using FluentAssertions;


namespace Core.DistributedCache.UnitTests.Behaviors;

public class RemoveAsyncBehaviorTests : MemoryCacheTestBase
{
    [Fact]
    public async Task RemoveAsync_WhenKeyExists_RemovesEntryFromCache()
    {
        // Arrange
        var cache = CreateCache();

        const string key = "users:1";

        var expected = new UserDto
        {
            Id = 1,
            Name = "Juan"
        };

        await cache.SetAsync(key, expected, TimeSpan.FromMinutes(5), ["user"], TestContext.Current.CancellationToken);

        // Act
        await cache.RemoveAsync(key, TestContext.Current.CancellationToken);

        // Assert
        var result = await cache.GetAsync<UserDto>(key, TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveAsync_WhenKeyDoesNotExist_ShouldNotThrow()
    {
        var cache = CreateCache();

        var action = async () =>
        {
            await cache.RemoveAsync(
                "customer:500",
                TestContext.Current.CancellationToken);
        };

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RemoveAsync_WhenKeyDoesNotExistInTags_ShouldNotThrow()
    {
        var cache = CreateCache();

        const string key = "users:2";

        var expected = new UserDto
        {
            Id = 1,
            Name = "Juan"
        };

        await cache.SetAsync(key, expected, TimeSpan.FromMinutes(5), [], TestContext.Current.CancellationToken);

        // Act
        await cache.RemoveAsync(key, TestContext.Current.CancellationToken);

    }


    private sealed class UserDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }
}