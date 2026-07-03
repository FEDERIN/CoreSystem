using FluentAssertions;

namespace Core.DistributedCache.UnitTests.Behaviors;

public class GetAsyncBehaviorTests : MemoryCacheTestBase
{
    [Fact]
    public async Task GetAsync_Should_Return_PreviouslyStored_Object_FromMemory()
    {
        // Arrange
        var cache = CreateCache();

        var expected = new User
        {
            Id = 1,
            Name = "Juan"
        };

        // Act
        await cache.SetAsync("user:1", expected, TimeSpan.FromMinutes(5), ["user"], TestContext.Current.CancellationToken);

        var result = await cache.GetAsync<User>("user:1", TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAsync_WhenKeyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var cache = CreateCache();

        var result = await cache.GetAsync<CustomerDto>(
            "customer:999",
            TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_AfterExpiration_ShouldReturnNull()
    {
        var cache = CreateCache();

        await cache.SetAsync(
            "customer:1",
            new CustomerDto(),
            TimeSpan.FromMilliseconds(100),
            ["customers"],
            TestContext.Current.CancellationToken);

        await Task.Delay(300, TestContext.Current.CancellationToken);

        var result = await cache.GetAsync<CustomerDto>(
            "customer:1",
            TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    private sealed class User
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }
}