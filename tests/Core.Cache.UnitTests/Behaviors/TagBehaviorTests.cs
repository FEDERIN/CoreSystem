using FluentAssertions;

namespace Core.Cache.UnitTests.Behaviors;

public sealed class TagBehaviorTests : MemoryCacheTestBase
{
    [Fact]
    public async Task InvalidateByTagAsync_Removes_AllEntries_Associated_With_Tag()
    {
        // Arrange
        var cache = CreateCache();

        var customer = new CustomerDto
        {
            Id = 1,
            Name = "John"
        };

        var order = new OrderDto
        {
            Id = 10,
            Number = "ORD-001"
        };

        await cache.SetAsync(
            "customer:1",
            customer,
            TimeSpan.FromMinutes(5),
            tags: ["customers"],
            TestContext.Current.CancellationToken);

        await cache.SetAsync(
            "order:10",
            order,
            TimeSpan.FromMinutes(5),
            tags: ["orders", "customers"],
            TestContext.Current.CancellationToken);

        // Act

        await cache.InvalidateByTagAsync("customers", TestContext.Current.CancellationToken);

        // Assert

        var customerResult =
            await cache.GetAsync<CustomerDto>("customer:1", TestContext.Current.CancellationToken);

        var orderResult =
            await cache.GetAsync<OrderDto>("order:10", TestContext.Current.CancellationToken);

        customerResult.Should().BeNull();
        orderResult.Should().BeNull();
    }

    [Fact]
    public async Task InvalidateByTagAsync_WhenTagDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        var cache = CreateCache();

        // Act
        var action = async () =>
        {
            await cache.InvalidateByTagAsync(
                "tag-that-does-not-exist",
                TestContext.Current.CancellationToken);
        };

        // Assert
        await action.Should().NotThrowAsync();
    }
}