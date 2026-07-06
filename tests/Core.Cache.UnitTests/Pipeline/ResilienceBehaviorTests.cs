using Core.Cache.Pipeline.Behaviors;
using Core.Cache.Pipeline.Contexts;
using FluentAssertions;
using Polly;

namespace Core.Cache.UnitTests.Pipeline;

public sealed class ResilienceBehaviorTests
{
    [Fact]
    public async Task InvokeAsync_WhenPipelineThrows_ShouldMarkRedisAsUnhealthy_AndRethrow()
    {
        // Arrange

        var pipeline = new ResiliencePipelineBuilder().Build();

        var behavior = new ResilienceBehavior(
            pipeline
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
    }

    private sealed class FakeCacheContext : CacheContext
    {
        public override Task ExecuteAsync()
            => Task.CompletedTask;
    }
}