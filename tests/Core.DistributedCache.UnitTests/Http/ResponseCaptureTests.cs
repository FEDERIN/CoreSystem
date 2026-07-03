using Core.DistributedCache.Http.Caching;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.DistributedCache.UnitTests.Http;

public sealed class ResponseCaptureTests
{
    private readonly ResponseCapture _sut = new();

    [Fact]
    public async Task CaptureAsync_ShouldCaptureResponseBody()
    {
        // Arrange
        var context = new DefaultHttpContext();

        await using var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        static async Task next(HttpContext ctx)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes("Hello World");

            await ctx.Response.Body.WriteAsync(bytes);
        }

        // Act
        var result = await _sut.CaptureAsync(context, next);

        // Assert
        System.Text.Encoding.UTF8.GetString(result.Body)
            .Should()
            .Be("Hello World");
    }

    [Fact]
    public async Task CaptureAsync_ShouldCaptureStatusCode()
    {
        // Arrange
        var context = new DefaultHttpContext();

        await using var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        static Task next(HttpContext ctx)
        {
            ctx.Response.StatusCode = StatusCodes.Status201Created;

            return Task.CompletedTask;
        }

        // Act
        var result = await _sut.CaptureAsync(
            context, next);

        // Assert
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Fact]
    public async Task CaptureAsync_ShouldCaptureAllowedHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();

        await using var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        static async Task next(HttpContext ctx)
        {
            ctx.Response.Headers.ETag = "\"12345\"";
            ctx.Response.Headers.CacheControl = "public,max-age=60";
            ctx.Response.Headers.ContentType = "application/json";

            await Task.CompletedTask;
        }

        // Act
        var result = await _sut.CaptureAsync(
            context,
            next);

        // Assert
        result.Headers.Should().ContainKey(HeaderNames.ETag);

        result.Headers.Should().ContainKey(HeaderNames.CacheControl);

        result.Headers.Should().ContainKey(HeaderNames.ContentType);
    }

    [Fact]
    public async Task CaptureAsync_ShouldIgnoreRestrictedHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();

        await using var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        static Task next(HttpContext ctx)
        {
            ctx.Response.Headers.TransferEncoding = "chunked";
            ctx.Response.Headers.Connection = "keep-alive";
            ctx.Response.Headers.Server = "Kestrel";
            ctx.Response.Headers.Date = DateTimeOffset.UtcNow.ToString("R");
            ctx.Response.Headers.ContentLength = 150;

            return Task.CompletedTask;
        }

        // Act
        var result = await _sut.CaptureAsync(context, next);

        // Assert
        result.Headers.Should().NotContainKey(HeaderNames.TransferEncoding);

        result.Headers.Should().NotContainKey(HeaderNames.Connection);

        result.Headers.Should().NotContainKey(HeaderNames.Server);

        result.Headers.Should().NotContainKey(HeaderNames.Date);

        result.Headers.Should().NotContainKey(HeaderNames.ContentLength);
    }

    [Fact]
    public async Task CaptureAsync_ShouldRestoreOriginalResponseStream()
    {
        // Arrange
        var context = new DefaultHttpContext();

        await using var originalStream = new MemoryStream();

        context.Response.Body = originalStream;

        static async Task next(HttpContext ctx)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes("Response");

            await ctx.Response.Body.WriteAsync(bytes);
        }

        // Act
        await _sut.CaptureAsync(context, next);

        // Assert
        context.Response.Body.Should().BeSameAs(originalStream);

        originalStream.Position = 0;

        using var reader = new StreamReader(originalStream);

        var body = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);

        body.Should().Be("Response");
    }
}