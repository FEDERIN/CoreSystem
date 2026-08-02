using Core.Http.Responses;
using Microsoft.AspNetCore.Http;

namespace Core.Http.UnitTests.Response;

public sealed class HttpResponseWriterTests
{
    private readonly HttpResponseWriter _writer = new();

    [Fact]
    public async Task WriteAsync_Should_Write_Status_Code()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.Body = new MemoryStream();

        var response = new CapturedResponse
        {
            StatusCode = StatusCodes.Status201Created,
            Body = [],
            ContentType = "application/json",
            Headers = new Dictionary<string, string[]>()
        };

        // Act
        await _writer.WriteAsync(context, response, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(
            StatusCodes.Status201Created,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task WriteAsync_Should_Write_Response_Body()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.Body = new MemoryStream();

        byte[] body = "Hello World"u8.ToArray();

        var response = new CapturedResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Body = body,
            ContentType = "application/json",
            Headers = new Dictionary<string, string[]>()
        };

        // Act
        await _writer.WriteAsync(context, response, TestContext.Current.CancellationToken);

        // Assert
        context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);

        string content = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);

        Assert.Equal("Hello World", content);
    }

    [Fact]
    public async Task WriteAsync_Should_Write_Response_Headers()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.Body = new MemoryStream();

        var response = new CapturedResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Body = [],
            ContentType = "application/json",
            Headers = new Dictionary<string, string[]>
            {
                ["x-test"] = ["value"]
            }
        };

        // Act
        await _writer.WriteAsync(context, response, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(
            "value",
            context.Response.Headers["x-test"]);
    }

    [Fact]
    public async Task WriteAsync_Should_Set_Content_Length()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.Body = new MemoryStream();

        byte[] body = "CoreSystem"u8.ToArray();

        var response = new CapturedResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Body = body,
            ContentType = "application/json",
            Headers = new Dictionary<string, string[]>()
        };

        // Act
        await _writer.WriteAsync(context, response, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(
            body.Length,
            context.Response.ContentLength);
    }

    [Fact]
    public async Task WriteAsync_Should_Not_Write_Body_For_Head_Request()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Request.Method = HttpMethods.Head;

        context.Response.Body = new MemoryStream();

        byte[] body = "Hello World"u8.ToArray();

        var response = new CapturedResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Body = body,
            ContentType = "application/json",
            Headers = new Dictionary<string, string[]>()
        };

        // Act
        await _writer.WriteAsync(context, response, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(
            0,
            context.Response.Body.Length);
    }
}