using Core.Http.Responses;
using Microsoft.AspNetCore.Http;

namespace Core.Http.UnitTests.Response;

public class ResponseCaptureTests
{
    private readonly ResponseCapture _sut = new();

    [Fact]
    public async Task CaptureAsync_ShouldCaptureResponseBody()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var response = await _sut.CaptureAsync(
            context,
            async ctx =>
            {
                await ctx.Response.WriteAsync("Hello World");
            });

        // Assert
        Assert.Equal(
            "Hello World",
            System.Text.Encoding.UTF8.GetString(response.Body));
    }

    [Fact]
    public async Task CaptureAsync_ShouldCaptureStatusCode()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var response = await _sut.CaptureAsync(
            context,
            ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status201Created;
                return Task.CompletedTask;
            });

        // Assert
        Assert.Equal(
            StatusCodes.Status201Created,
            response.StatusCode);
    }

    [Fact]
    public async Task CaptureAsync_ShouldCaptureHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var response = await _sut.CaptureAsync(
            context,
            ctx =>
            {
                ctx.Response.Headers["X-Test"] = "Value";
                return Task.CompletedTask;
            });

        // Assert
        Assert.True(response.Headers.ContainsKey("X-Test"));
        Assert.Equal(
            "Value",
            response.Headers["X-Test"].Single());
    }

    [Fact]
    public async Task CaptureAsync_ShouldIgnoreRestrictedHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var response = await _sut.CaptureAsync(
            context,
            ctx =>
            {
                ctx.Response.Headers.ContentLength = 10;
                ctx.Response.Headers.TransferEncoding = "chunked";

                return Task.CompletedTask;
            });

        // Assert
        Assert.DoesNotContain(
            "Content-Length",
            response.Headers.Keys);

        Assert.DoesNotContain(
            "Transfer-Encoding",
            response.Headers.Keys);
    }

    [Fact]
    public async Task CaptureAsync_ShouldRestoreOriginalResponseBody()
    {
        // Arrange
        var context = new DefaultHttpContext();

        var originalBody = context.Response.Body;

        // Act
        await _sut.CaptureAsync(
            context,
            async ctx =>
            {
                await ctx.Response.WriteAsync("Hello");
            });

        // Assert
        Assert.Same(
            originalBody,
            context.Response.Body);
    }

    [Fact]
    public async Task CaptureAsync_ShouldCopyResponseToOriginalStream()
    {
        // Arrange
        var context = new DefaultHttpContext();

        using var original = new MemoryStream();

        context.Response.Body = original;

        // Act
        await _sut.CaptureAsync(
            context,
            async ctx =>
            {
                await ctx.Response.WriteAsync("Hello");
            });

        // Assert
        original.Position = 0;

        using var reader = new StreamReader(original);

        var body = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);

        Assert.Equal(
            "Hello",
            body);
    }

    [Fact]
    public async Task CaptureAsync_ShouldCaptureEmptyBody()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var response = await _sut.CaptureAsync(
            context,
            _ => Task.CompletedTask);

        // Assert
        Assert.Empty(response.Body);
    }

    [Fact]
    public async Task CaptureAsync_ShouldCaptureMultipleHeaderValues()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var response = await _sut.CaptureAsync(
            context,
            ctx =>
            {
                ctx.Response.Headers.Append(
                    "X-Test",
                    "Value1");

                ctx.Response.Headers.Append(
                    "X-Test",
                    "Value2");

                return Task.CompletedTask;
            });

        // Assert
        Assert.Equal(
            2,
            response.Headers["X-Test"].Length);

        Assert.Contains(
            "Value1",
            response.Headers["X-Test"]);

        Assert.Contains(
            "Value2",
            response.Headers["X-Test"]);
    }
}