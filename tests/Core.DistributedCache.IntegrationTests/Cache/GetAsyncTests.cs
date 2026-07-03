using Core.DistributedCache.IntegrationTests.Fixtures;
using FluentAssertions;
using StackExchange.Redis;

namespace Core.DistributedCache.IntegrationTests.Cache;

public sealed class GetAsyncTests(RedisContainerFixture fixture)
    : RedisCacheTestBase(fixture),
      IClassFixture<RedisContainerFixture>
{
    [Fact]
    public async Task GetAsync_WhenKeyExists_ShouldReturnStoredObject()
    {
        // Arrange
        var expected = new CustomerDto
        {
            Id = 1,
            Name = "Juan"
        };

        await Cache.SetAsync(
            "customer:1",
            expected,
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        // Act
        var result = await Cache.GetAsync<CustomerDto>("customer:1", TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAsync_WhenKeyDoesNotExist_ShouldReturnNull()
    {
        var result =
            await Cache.GetAsync<CustomerDto>("customer:999", TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WhenEntryExpires_ShouldReturnNull()
    {
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

        var result =
            await Cache.GetAsync<CustomerDto>("customer:1", TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldPersistValueInRedis()
    {
        var customer = new CustomerDto
        {
            Id = 5,
            Name = "Pedro"
        };

        await Cache.SetAsync(
            "customer:5",
            customer,
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        RedisValue value =
            await Database.StringGetAsync(
                BuildRedisKey("customer:5"));

        value.HasValue.Should().BeTrue();
    }
}