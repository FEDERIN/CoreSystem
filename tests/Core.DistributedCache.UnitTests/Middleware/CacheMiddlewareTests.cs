using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Core.DistributedCache.UnitTests.Middleware;

public sealed class CacheMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldDelegateToHttpCacheHandler()
    {
        // Arrange
        var context = new DefaultHttpContext();

        RequestDelegate next = _ => Task.CompletedTask;

        var handler = new Mock<IHttpCacheHandler>();

        var middleware = new CacheMiddleware(
            next,
            handler.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        handler.Verify(h =>
            h.HandleAsync(
                context,
                next),
            Times.Once);
    }
}