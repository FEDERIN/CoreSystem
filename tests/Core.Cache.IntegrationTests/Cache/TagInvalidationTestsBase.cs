using Core.Cache.Abstractions;
using FluentAssertions;

namespace Core.Cache.IntegrationTests.Cache;

public abstract class TagInvalidationTestsBase
{
    protected abstract ICoreCache Cache { get; }

    [Fact]
    public async Task InvalidateByTagAsync_ShouldRemoveAllEntriesAssociatedWithTag()
    {
        // Arrange
        await Cache.SetAsync(
            "customer:1",
            new CustomerDto { Id = 1, Name = "John" },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        await Cache.SetAsync(
            "customer:2",
            new CustomerDto { Id = 2, Name = "Peter" },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        // Act
        await Cache.InvalidateByTagAsync(
            "customers",
            TestContext.Current.CancellationToken);

        // Assert
        (await Cache.GetAsync<CustomerDto>("customer:1", TestContext.Current.CancellationToken))
            .Should().BeNull();

        (await Cache.GetAsync<CustomerDto>("customer:2", TestContext.Current.CancellationToken))
            .Should().BeNull();
    }

    [Fact]
    public async Task InvalidateByTagAsync_ShouldOnlyRemoveEntriesWithSpecifiedTag()
    {
        await Cache.SetAsync(
            "customer:1",
            new CustomerDto { Id = 1, Name = "John" },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        await Cache.SetAsync(
            "order:1",
            new OrderDto { Id = 10, Number = "ORD-001" },
            TimeSpan.FromMinutes(5),
            ["orders"],
            TestContext.Current.CancellationToken);

        await Cache.InvalidateByTagAsync(
            "customers",
            TestContext.Current.CancellationToken);

        (await Cache.GetAsync<CustomerDto>("customer:1", TestContext.Current.CancellationToken))
            .Should().BeNull();

        (await Cache.GetAsync<OrderDto>("order:1", TestContext.Current.CancellationToken))
            .Should().NotBeNull();
    }

    [Fact]
    public async Task InvalidateByTagAsync_WhenEntryHasMultipleTags_ShouldRemoveEntry()
    {
        await Cache.SetAsync(
            "customer:1",
            new CustomerDto { Id = 1, Name = "John" },
            TimeSpan.FromMinutes(5),
            ["customers", "vip"],
            TestContext.Current.CancellationToken);

        await Cache.InvalidateByTagAsync(
            "vip",
            TestContext.Current.CancellationToken);

        (await Cache.GetAsync<CustomerDto>("customer:1", TestContext.Current.CancellationToken))
            .Should().BeNull();
    }

    [Fact]
    public async Task InvalidateByTagAsync_WhenTagDoesNotExist_ShouldNotThrow()
    {
        var action = async () =>
            await Cache.InvalidateByTagAsync(
                "unknown-tag",
                TestContext.Current.CancellationToken);

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task InvalidateByTagAsync_CanBeCalledMultipleTimes()
    {
        await Cache.SetAsync(
            "customer:1",
            new CustomerDto { Id = 1, Name = "John" },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        await Cache.InvalidateByTagAsync(
            "customers",
            TestContext.Current.CancellationToken);

        var action = async () =>
            await Cache.InvalidateByTagAsync(
                "customers",
                TestContext.Current.CancellationToken);

        await action.Should().NotThrowAsync();
    }
}