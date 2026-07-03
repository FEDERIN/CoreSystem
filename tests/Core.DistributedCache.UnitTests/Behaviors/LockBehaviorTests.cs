using FluentAssertions;

namespace Core.DistributedCache.UnitTests.Behaviors;

public sealed class LockBehaviorTests : MemoryCacheTestBase
{
    [Fact]
    public async Task GetOrAddAsync_WhenCalledConcurrently_ShouldExecuteFactoryOnlyOnce()
    {
        // Arrange
        var cache = CreateCache();

        var executions = 0;

        async Task<string> Factory(CancellationToken ct)
        {
            Interlocked.Increment(ref executions);

            await Task.Delay(100, ct);

            return Guid.NewGuid().ToString();
        }

        // Act
        var tasks = Enumerable
            .Range(0, 25)
            .Select(_ => cache.GetOrAddAsync(
                key: "customer:1",
                factory: Factory));

        var results = await Task.WhenAll(tasks);

        // Assert

        executions.Should().Be(1);

        results.Distinct().Should().ContainSingle();
    }
}