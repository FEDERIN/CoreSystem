using Core.Cache.Http.Caching;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.Cache.UnitTests.Http;

public sealed class DefaultResponseCachePolicyTests
{
    private readonly DefaultResponseCachePolicy _policy = new();

    [Theory]
    [InlineData(StatusCodes.Status200OK, true)]
    [InlineData(StatusCodes.Status404NotFound, false)]
    public void CanCache_WhenStatusCodeIsEvaluated_ShouldReturnExpectedResult(
    int statusCode,
    bool expected)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.StatusCode = statusCode;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().Be(expected);
    }


    [Fact]
    public void CanCache_WhenResponseContainsSetCookie_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers.Append(
            HeaderNames.SetCookie,
            "session=abc");

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    
    [Theory]
    [InlineData("private", false)]
    [InlineData("no-store", false)]
    [InlineData("public,max-age=60", true)]
    public void CanCache_WhenCacheControlHeaderIsEvaluated_ShouldReturnExpectedResult(string value, bool should)
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers.Append(
            HeaderNames.CacheControl,
            value);

        // Act
        var result = _policy.CanCache(context);

        // Assert

        if(should)
            result.Should().BeTrue();
        else
            result.Should().BeFalse();
    }


    [Fact]
    public void CanCache_WhenCacheControlHeaderDoesNotExist_ShouldReturnTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.StatusCode = StatusCodes.Status200OK;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeTrue();
    }
}