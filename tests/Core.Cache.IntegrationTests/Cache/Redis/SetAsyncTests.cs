using Core.Cache.IntegrationTests.Fixtures;
using FluentAssertions;

namespace Core.Cache.IntegrationTests.Cache.Redis;

public sealed class SetAsyncTests(
RedisContainerFixture fixture)
        : RedisCacheTestBase(fixture),
      IClassFixture<RedisContainerFixture>
{
    [Fact]
    public async Task SetAsync_WhenValueIsStored_ShouldBeRetrievedFromRedis()
    {
        // Arrange
        var customer = new CustomerDto
        {
            Id = 1,
            Name = "Juan"
        };

        // Act
        await Cache.SetAsync(
            "customer:1",
            customer,
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        var result = await Cache.GetAsync<CustomerDto>(
            "customer:1",
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(customer);
    }

    [Fact]
    public async Task SetAsync_WhenKeyAlreadyExists_ShouldOverwritePreviousValue()
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
        await Cache.SetAsync(
            "customer:1",
            new CustomerDto
            {
                Id = 1,
                Name = "Pedro"
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        var result = await Cache.GetAsync<CustomerDto>(
            "customer:1",
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Pedro");
    }

    [Fact]
    public async Task SetAsync_WhenExpirationIsSpecified_ShouldExpireKey()
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

        // Act
        await Task.Delay(600, TestContext.Current.CancellationToken);

        var result = await Cache.GetAsync<CustomerDto>(
            "customer:1",
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WhenStoringCollection_ShouldRetrieveCollection()
    {
        // Arrange
        var customers = new List<CustomerDto>
        {
            new() { Id = 1, Name = "Juan" },
            new() { Id = 2, Name = "Pedro" }
        };

        // Act
        await Cache.SetAsync(
            "customers",
            customers,
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        var result = await Cache.GetAsync<List<CustomerDto>>(
            "customers",
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeEquivalentTo(customers);
    }

    [Fact]
    public async Task SetAsync_WhenStoringDictionary_ShouldRetrieveDictionary()
    {
        // Arrange
        var dictionary = new Dictionary<string, CustomerDto>
        {
            ["1"] = new() { Id = 1, Name = "Juan" },
            ["2"] = new() { Id = 2, Name = "Pedro" }
        };

        // Act
        await Cache.SetAsync(
            "dictionary",
            dictionary,
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        var result = await Cache.GetAsync<Dictionary<string, CustomerDto>>(
            "dictionary",
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeEquivalentTo(dictionary);
    }

    [Fact]
    public async Task SetAsync_WhenStoringPrimitive_ShouldRetrievePrimitive()
    {
        // Arrange
        const string expected = "Hello Redis";

        // Act
        await Cache.SetAsync(
            "message",
            expected,
            TimeSpan.FromMinutes(5),
            null,
            TestContext.Current.CancellationToken);

        var result = await Cache.GetAsync<string>(
            "message",
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().Be(expected);
    }

    private sealed class CustomerDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }
}