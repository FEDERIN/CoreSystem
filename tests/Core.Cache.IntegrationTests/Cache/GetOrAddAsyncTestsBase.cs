using Core.Cache.Abstractions;
using FluentAssertions;

namespace Core.Cache.IntegrationTests.Cache;

public abstract class GetOrAddAsyncTestsBase
{
    protected abstract ICoreCache Cache { get; }
    
    [Fact]
    public async Task GetOrAddAsync_WhenKeyDoesNotExist_ShouldExecuteFactoryAndStoreValue()
    {
        // Arrange
        var executions = 0;

        // Act
        var result = await Cache.GetOrAddAsync(
            "customer:15",
            ct =>
            {
                executions++;

                return Task.FromResult(new CustomerDto
                {
                    Id = 1,
                    Name = "Juan"
                });
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        var cached = await Cache.GetAsync<CustomerDto>(
            "customer:15",
            TestContext.Current.CancellationToken);

        // Assert
        executions.Should().Be(1);

        result.Should().NotBeNull();

        result.Should().BeEquivalentTo(cached);

        //var exists = await Database.KeyExistsAsync("customer:15");

        //exists.Should().BeTrue();
    }

    [Fact]
    public async Task GetOrAddAsync_WhenKeyAlreadyExists_ShouldReturnCachedValue()
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

        var executions = 0;

        // Act
        var result = await Cache.GetOrAddAsync(
            "customer:1",
            ct =>
            {
                executions++;

                return Task.FromResult(new CustomerDto
                {
                    Id = 999,
                    Name = "Pedro"
                });
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        // Assert
        executions.Should().Be(0);

        result.Should().NotBeNull();

        result!.Name.Should().Be("Juan");
    }

    [Fact]
    public async Task GetOrAddAsync_WhenFactoryReturnsNull_ShouldReturnNull()
    {
        // Arrange
        var executions = 0;

        // Act
        var result = await Cache.GetOrAddAsync<CustomerDto?>(
            "customer:16",
            ct =>
            {
                executions++;

                return Task.FromResult<CustomerDto?>(null);
            },
            TimeSpan.FromMinutes(5),
            ["customers"],
            TestContext.Current.CancellationToken);

        // Assert
        executions.Should().Be(1);

        result.Should().BeNull();

        var cached = await Cache.GetAsync<CustomerDto>(
            "customer:16",
            TestContext.Current.CancellationToken);

        cached.Should().BeNull();
    }

    [Fact]
    public async Task GetOrAddAsync_WhenEntryExpires_ShouldExecuteFactoryAgain()
    {
        // Arrange
        var executions = 0;

        async Task<CustomerDto> Factory(CancellationToken ct)
        {
            executions++;

            return new CustomerDto
            {
                Id = executions,
                Name = $"Customer {executions}"
            };
        }

        await Cache.GetOrAddAsync(
            "customer:20",
            Factory,
            TimeSpan.FromMilliseconds(300),
            ["customers"],
            TestContext.Current.CancellationToken);

        await Task.Delay(600, TestContext.Current.CancellationToken);

        // Act
        var result = await Cache.GetOrAddAsync(
            "customer:20",
            Factory,
            TimeSpan.FromMilliseconds(300),
            ["customers"],
            TestContext.Current.CancellationToken);

        // Assert
        executions.Should().Be(2);

        result!.Id.Should().Be(2);

        //var ttl = await Database.KeyTimeToLiveAsync("customer:20");

        //ttl.Should().NotBeNull();
        //ttl.Value.Should().BeLessThan(TimeSpan.FromMilliseconds(300));
    }

    [Fact]
    public async Task GetOrAddAsync_WhenCalledConcurrently_ShouldExecuteFactoryOnlyOnce()
    {
        // Arrange
        var executions = 0;

        async Task<CustomerDto> Factory(CancellationToken ct)
        {
            Interlocked.Increment(ref executions);

            await Task.Delay(200, ct);

            return new CustomerDto
            {
                Id = 1,
                Name = Guid.NewGuid().ToString()
            };
        }

        // Act
        var tasks = Enumerable.Range(0, 25)
            .Select(_ => Cache.GetOrAddAsync(
                "customer:30",
                Factory,
                TimeSpan.FromMinutes(5),
                ["customers"],
                TestContext.Current.CancellationToken));

        var results = await Task.WhenAll(tasks);

        // Assert
        executions.Should().Be(1);

        results
            .Select(r => r!.Name)
            .Distinct()
            .Should()
            .ContainSingle();
    }
}