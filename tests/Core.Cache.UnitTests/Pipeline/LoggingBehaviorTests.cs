using Core.Cache.Pipeline.Behaviors;
using Core.Cache.Pipeline.Contexts;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Core.Cache.UnitTests.Pipeline;

public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task InvokeAsync_WhenNextThrows_ShouldRethrowException()
    {
        // Arrange
        var logger = NullLogger<LoggingBehavior>.Instance;

        var behavior = new LoggingBehavior(logger);

        var context = new FakeCacheContext
        {
            Key = "customer:1",
            Storage = null!,
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