using FluentAssertions;

namespace Core.Cache.UnitTests.Behaviors;

public class GetOrAddBehaviorTests : MemoryCacheTestBase
{
    [Fact]
    public async Task GetOrAddAsync_WhenKeyDoesNotExist_ShouldExecuteFactoryAndStoreValue()
    {
        // Arrange
        var cache = CreateCache();

        var factoryCalls = 0;

        // Act
        var result = await cache.GetOrAddAsync<TestUser>(
            "users:1",
            ct =>
            {
                factoryCalls++;

                return Task.FromResult(new TestUser
                {
                    Id = 2,
                    Name = "Pedro"
                });
            },
            TimeSpan.FromMinutes(5),
            ["user"],
            TestContext.Current.CancellationToken);

        var cached = await cache.GetAsync<TestUser>("users:1", TestContext.Current.CancellationToken);


        // Assert
        result.Should().NotBeNull();

        result!.Name.Should().Be("Pedro");

        cached.Should().NotBeNull();

        cached!.Name.Should().Be("Pedro");

        factoryCalls.Should().Be(1);
    }

    [Fact]
    public async Task GetOrAddAsync_WhenKeyAlreadyExists_ShouldReturnCachedValue_WithoutExecutingFactory()
    {
        // Arrange
        var cache = CreateCache();

        var expected = new TestUser
        {
            Id = 1,
            Name = "Juan"
        };

        await cache.SetAsync(
            "users:1",
            expected,
            TimeSpan.FromMinutes(5),
            ["users"],
            TestContext.Current.CancellationToken);

        var factoryCalls = 0;

        // Act
        var result = await cache.GetOrAddAsync<TestUser>(
            "users:1",
            ct =>
            {
                factoryCalls++;

                return Task.FromResult(new TestUser
                {
                    Id = 999,
                    Name = "Pedro"
                });
            },
            TimeSpan.FromMinutes(5),
            ["users"],
            TestContext.Current.CancellationToken);

        // Assert
        factoryCalls.Should().Be(0);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetOrAddAsync_WhenFactoryReturnsNull_ShouldReturnNull_AndNotCacheValue()
    {
        // Arrange
        var cache = CreateCache();

        var executions = 0;

        // Act
        var result = await cache.GetOrAddAsync<TestUser?>(
            "users:1",
            ct =>
            {
                executions++;
                return Task.FromResult<TestUser?>(null);
            },
            TimeSpan.FromMinutes(5),
            ["users"],
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();

        executions.Should().Be(1);

        var cached = await cache.GetAsync<TestUser>(
            "users:1",
            TestContext.Current.CancellationToken);

        cached.Should().BeNull();
    }
}

internal sealed class TestUser
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;
}