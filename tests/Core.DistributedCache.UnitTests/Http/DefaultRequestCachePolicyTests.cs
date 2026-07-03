using Core.Cache.Http.Caching;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.Cache.UnitTests.Http;

public sealed class DefaultRequestCachePolicyTests
{
    private readonly DefaultRequestCachePolicy _policy = new();

    [Fact]
    public void CanCache_WhenRequestIsGet_ShouldReturnTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanCache_WhenRequestIsHead_ShouldReturnTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Head;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanCache_WhenRequestIsPost_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCache_WhenRequestIsPut_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Put;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCache_WhenRequestIsDelete_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Delete;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCache_WhenAuthorizationHeaderExists_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Headers[HeaderNames.Authorization] = "Bearer token";

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCache_WhenHeadRequestContainsAuthorizationHeader_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Head;
        context.Request.Headers[HeaderNames.Authorization] = "Bearer token";

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }
}