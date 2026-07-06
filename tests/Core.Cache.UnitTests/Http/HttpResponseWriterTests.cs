using Core.Cache.Http.Caching;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Core.Cache.UnitTests.Http;

public sealed class HttpResponseWriterTests
{
    private readonly HttpResponseWriter _writer = new();

    [Fact]
    public async Task WriteAsync_ShouldWriteStatusCodeHeadersAndBody()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.Body = new MemoryStream();

        var expectedBody = "Hello World"u8.ToArray();

        var cached = new CachedHttpResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Body = expectedBody,
            Headers = new Dictionary<string, string[]>
            {
                ["Content-Type"] = ["application/json"],
                ["ETag"] = ["12345"]
            }
        };

        // Act
        await _writer.WriteAsync(context, cached);

        // Assert

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        context.Response.Headers.ContentType
            .ToString()
            .Should()
            .Be("application/json");

        context.Response.Headers.ETag
            .ToString()
            .Should()
            .Be("12345");

        context.Response.ContentLength.Should().Be(expectedBody.Length);

        context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);

        var body = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);

        body.Should().Be("Hello World");
    }

    [Fact]
    public async Task WriteAsync_WhenRequestIsHead_ShouldNotWriteBody()
    {
        // Arrange

        var context = new DefaultHttpContext();

        context.Request.Method = HttpMethods.Head;

        context.Response.Body = new MemoryStream();

        var cached = new CachedHttpResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Body = "Hello"u8.ToArray(),
            Headers = new Dictionary<string, string[]>
            {
                ["Content-Type"] = ["text/plain"]
            }
        };

        // Act

        await _writer.WriteAsync(context, cached);

        // Assert

        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);

        context.Response.Body.Length.Should().Be(0);

        context.Response.ContentLength.Should().BeNull();
    }

    [Fact]
    public async Task WriteAsync_WhenResponseHasNoHeaders_ShouldStillWriteBody()
    {
        // Arrange

        var context = new DefaultHttpContext();

        context.Response.Body = new MemoryStream();

        var cached = new CachedHttpResponse
        {
            StatusCode = StatusCodes.Status201Created,
            Body = "Created"u8.ToArray(),
            Headers = []
        };

        // Act

        await _writer.WriteAsync(context, cached);

        // Assert

        context.Response.StatusCode.Should().Be(StatusCodes.Status201Created);

        context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);

        var body = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);

        body.Should().Be("Created");
    }

    [Fact]
    public async Task WriteAsync_ShouldOverwriteExistingHeaders()
    {
        // Arrange

        var context = new DefaultHttpContext();

        context.Response.Body = new MemoryStream();

        context.Response.Headers.ContentType = "text/plain";

        var cached = new CachedHttpResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Body = "Body"u8.ToArray(),
            Headers = new Dictionary<string, string[]>
            {
                ["Content-Type"] = ["application/json"]
            }
        };

        // Act

        await _writer.WriteAsync(context, cached);

        // Assert

        context.Response.Headers.ContentType.ToString()
            .Should()
            .Be("application/json");
    }
}