using FluentAssertions;

namespace Core.DistributedCache.UnitTests.Behaviors;

public class SetAsyncBehaviorTests : MemoryCacheTestBase
{
    [Fact]
    public async Task SetAsync_WhenValueIsStored_CanBeRetrieved()
    {
        // Arrange
        var cache = CreateCache();

        var customer = new Customer
        {
            Id = 10,
            Name = "Juan"
        };

        // Act
        await cache.SetAsync("customer:10", customer,
            TimeSpan.FromMinutes(5),
            tags: ["customers"],
            TestContext.Current.CancellationToken);

        var result = await cache.GetAsync<Customer>("customer:10",
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(10);
        result.Name.Should().Be("Juan");
    }

    [Fact]
    public async Task SetAsync_WhenKeyAlreadyExists_ShouldOverwritePreviousValue()
    {
        var cache = CreateCache();

        await cache.SetAsync(
            "customer:1",
            new CustomerDto
            {
                Id = 1,
                Name = "Juan"
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        await cache.SetAsync(
            "customer:1",
            new CustomerDto
            {
                Id = 1,
                Name = "Pedro"
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        var result = await cache.GetAsync<CustomerDto>(
            "customer:1",
            TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Pedro");
    }

    private sealed class Customer
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }
}