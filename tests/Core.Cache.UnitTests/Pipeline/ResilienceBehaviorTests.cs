using Core.Cache.Abstractions;
using Core.Cache.Pipeline.Behaviors;
using Core.Cache.Pipeline.Contexts;
using FluentAssertions;
using Moq;
using Polly;

namespace Core.Cache.UnitTests.Pipeline;

public sealed class ResilienceBehaviorTests
{
    [Fact]
    public async Task InvokeAsync_WhenPipelineThrows_ShouldMarkRedisAsUnhealthy_AndRethrow()
    {
        // Arrange

        var pipeline = new ResiliencePipelineBuilder().Build();

        var healthState = new Mock<IHealthState>();

        var behavior = new ResilienceBehavior(
            pipeline,
            healthState.Object
        );

        var context = new FakeCacheContext
        {
            Key = "customer:1",
            CancellationToken = TestContext.Current.CancellationToken
        };

        static Task Next(CacheContext _)
            => throw new InvalidOperationException("Boom!");

        // Act

        var action = () => behavior.InvokeAsync(context, Next);

        // Assert

        await action
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Boom!");

        healthState.Verify(
            x => x.MarkUnhealthy(),
            Times.Once);

        healthState.Verify(
            x => x.MarkHealthy(),
            Times.Never);
    }

    private sealed class FakeCacheContext : CacheContext
    {
        public override Task ExecuteAsync()
            => Task.CompletedTask;
    }
}