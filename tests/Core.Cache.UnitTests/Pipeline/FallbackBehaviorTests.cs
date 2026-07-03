using Core.Cache.Abstractions;
using Core.Cache.Pipeline.Behaviors;
using Core.Cache.Pipeline.Contexts;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Core.Cache.UnitTests.Pipeline;

public sealed class FallbackBehaviorTests
{
    [Fact]
    public async Task InvokeAsync_WhenPrimaryFails_ShouldSwitchToFallback()
    {
        // Arrange
        var resolver = new Mock<ICacheStorageResolver>();

        var primary = new Mock<ICacheStorage>().Object;
        var fallback = new Mock<ICacheStorage>().Object;

        resolver.Setup(x => x.Primary).Returns(primary);
        resolver.Setup(x => x.Fallback).Returns(fallback);

        var behavior = new FallbackBehavior(
            resolver.Object,
            NullLogger<FallbackBehavior>.Instance);

        var context = new FakeCacheContext
        {
            Key = "customer:1",
            Storage = primary,
            CancellationToken = TestContext.Current.CancellationToken
        };

        var calls = 0;

        Task Next(CacheContext ctx)
        {
            calls++;

            if (calls == 1)
                throw new InvalidOperationException("Redis failed");

            return Task.CompletedTask;
        }

        // Act
        await behavior.InvokeAsync(context, Next);

        // Assert
        calls.Should().Be(2);

        context.Storage.Should().BeSameAs(fallback);

        context.Exception.Should().BeOfType<InvalidOperationException>();
    }

    private sealed class FakeCacheContext : CacheContext
    {
        public override Task ExecuteAsync()
            => Task.CompletedTask;
    }
}