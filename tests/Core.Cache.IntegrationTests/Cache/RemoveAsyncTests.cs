using Core.Cache.IntegrationTests.Fixtures;
using FluentAssertions;

namespace Core.Cache.IntegrationTests.Cache;

public sealed class RemoveAsyncTests(RedisContainerFixture fixture)
    : RedisCacheTestBase(fixture),
      IClassFixture<RedisContainerFixture>
{
    [Fact]
    public async Task RemoveAsync_WhenKeyExists_ShouldRemoveEntry()
    {
        // Arrange
        await Cache.SetAsync(
            "customer:25",
            new CustomerDto
            {
                Id = 1,
                Name = "Juan"
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        // Act
        await Cache.RemoveAsync(
            "customer:25",
            TestContext.Current.CancellationToken);

        // Assert
        var result = await Cache.GetAsync<CustomerDto>(
            "customer:25",
            TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveAsync_WhenKeyDoesNotExist_ShouldNotThrow()
    {
        // Act
        var action = async () =>
        {
            await Cache.RemoveAsync(
                "customer:999",
                TestContext.Current.CancellationToken);
        };

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RemoveAsync_WhenCalledTwice_ShouldNotThrow()
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

        await Cache.RemoveAsync(
            "customer:1",
            TestContext.Current.CancellationToken);

        // Act
        var action = async () =>
        {
            await Cache.RemoveAsync(
                "customer:1",
                TestContext.Current.CancellationToken);
        };

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveKeyFromExistsAsync()
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
        await Cache.RemoveAsync(
            "customer:1",
            TestContext.Current.CancellationToken);

        // Assert
        var exists = await Cache.ExistsAsync(
            "customer:1",
            TestContext.Current.CancellationToken);

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveEntryAssociatedWithTag()
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
        await Cache.RemoveAsync(
            "customer:1",
            TestContext.Current.CancellationToken);

        // Assert
        var action = async () =>
        {
            await Cache.InvalidateByTagAsync(
                "customers",
                TestContext.Current.CancellationToken);
        };

        await action.Should().NotThrowAsync();
    }
}