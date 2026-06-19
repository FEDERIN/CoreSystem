using Core.Idempotency.Middleware;
using Core.Idempotency.Models;
using Core.Idempotency.Tests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Core.Idempotency.Tests.Middleware;

public class IdempotencyMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WithDisabledIdempotency_CallsNextDelegate()
    {
        // Arrange
        var fixture = new IdempotencyMiddlewareFixture();
        fixture.Options.Enabled = false;
        var middleware = fixture.CreateMiddleware();
        fixture.HttpContext.Request.Method = "POST";

        // Act
        await middleware.InvokeAsync(fixture.HttpContext);

        // Assert
        fixture.NextMock.Verify(next => next(fixture.HttpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithNullStorage_CallsNextDelegate()
    {
        // Arrange
        var fixture = new IdempotencyMiddlewareFixture();
        var optionsWrapper = fixture.Options.AsOptions();
        var middleware = new IdempotencyMiddleware(fixture.NextMock.Object, optionsWrapper, fixture.Metrics, null);
        fixture.HttpContext.Request.Method = "POST";

        // Act
        await middleware.InvokeAsync(fixture.HttpContext);

        // Assert
        fixture.NextMock.Verify(next => next(fixture.HttpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithNonAllowedMethod_CallsNextDelegate()
    {
        // Arrange
        var fixture = new IdempotencyMiddlewareFixture();
        var middleware = fixture.CreateMiddleware();
        fixture.HttpContext.Request.Method = "GET";
        fixture.HttpContext.Request.Headers["X-Idempotency-Key"] = "test-key";

        // Act
        await middleware.InvokeAsync(fixture.HttpContext);

        // Assert
        fixture.NextMock.Verify(next => next(fixture.HttpContext), Times.Once);
        fixture.StorageMock.Verify(s => s.GetAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithoutIdempotencyKey_CallsNextDelegate()
    {
        // Arrange
        var fixture = new IdempotencyMiddlewareFixture();
        var middleware = fixture.CreateMiddleware();
        fixture.HttpContext.Request.Method = "POST";
        // No idempotency key header

        // Act
        await middleware.InvokeAsync(fixture.HttpContext);

        // Assert
        fixture.NextMock.Verify(next => next(fixture.HttpContext), Times.Once);
        fixture.StorageMock.Verify(s => s.GetAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithCachedResponse_ReturnsCachedResponse()
    {
        // Arrange
        var fixture = new IdempotencyMiddlewareFixture();
        var cachedResponse = new IdempotencyResponse
        {
            StatusCode = 200,
            ContentType = "application/json",
            Body = "{\"cached\":true}"
        };

        fixture.StorageMock
            .Setup(s => s.GetAsync("test-key"))
            .ReturnsAsync(cachedResponse);

        var middleware = fixture.CreateMiddleware();
        fixture.HttpContext.Request.Method = "POST";
        fixture.HttpContext.Request.Headers["X-Idempotency-Key"] = "test-key";

        // Act
        await middleware.InvokeAsync(fixture.HttpContext);

        // Assert
        fixture.HttpContext.Response.StatusCode.Should().Be(200);
        fixture.HttpContext.Response.Headers["X-Idempotency-Cache"].Should().Contain("HIT");
        fixture.NextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_WithoutCachedResponse_ExecutesNextAndSavesResponse()
    {
        // Arrange
        var fixture = new IdempotencyMiddlewareFixture();
        fixture.StorageMock
            .Setup(s => s.GetAsync(It.IsAny<string>()))
            .ReturnsAsync((IdempotencyResponse?)null);

        fixture.NextMock
            .Setup(n => n(It.IsAny<HttpContext>()))
            .Callback((HttpContext ctx) =>
            {
                ctx.Response.StatusCode = 201;
                ctx.Response.ContentType = "application/json";
            })
            .Returns(Task.CompletedTask);

        var middleware = fixture.CreateMiddleware();
        fixture.HttpContext.Request.Method = "POST";
        fixture.HttpContext.Request.Headers["X-Idempotency-Key"] = "test-key";

        // Act
        await middleware.InvokeAsync(fixture.HttpContext);

        // Assert
        fixture.NextMock.Verify(next => next(fixture.HttpContext), Times.Once);
        fixture.StorageMock.Verify(
            s => s.SaveAsync("test-key", It.IsAny<IdempotencyResponse>(), It.IsAny<TimeSpan>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithErrorResponse_DoesNotSaveResponse()
    {
        // Arrange
        var fixture = new IdempotencyMiddlewareFixture();
        fixture.StorageMock
            .Setup(s => s.GetAsync(It.IsAny<string>()))
            .ReturnsAsync((IdempotencyResponse?)null);

        fixture.NextMock
            .Setup(n => n(It.IsAny<HttpContext>()))
            .Callback((HttpContext ctx) =>
            {
                ctx.Response.StatusCode = 400; // Error response
            })
            .Returns(Task.CompletedTask);

        var middleware = fixture.CreateMiddleware();
        fixture.HttpContext.Request.Method = "POST";
        fixture.HttpContext.Request.Headers["X-Idempotency-Key"] = "test-key";

        // Act
        await middleware.InvokeAsync(fixture.HttpContext);

        // Assert
        fixture.StorageMock.Verify(
            s => s.SaveAsync(It.IsAny<string>(), It.IsAny<IdempotencyResponse>(), It.IsAny<TimeSpan>()),
            Times.Never);
    }
}