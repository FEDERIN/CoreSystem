using Core.Cache.Http.Caching;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.Cache.UnitTests.Http;

public sealed class DefaultRequestCachePolicyTests
{
    private readonly DefaultRequestCachePolicy _policy = new();

    [Theory]
    [InlineData("Get")]
    [InlineData("Head")]
    public void CanCache_WhenRequestMethodIsSupported_ShouldReturnTrue(string method)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.GetCanonicalizedValue(method);

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Post")]
    [InlineData("Put")]
    [InlineData("Delete")]
    public void CanCache_WhenRequestMethodIsNotSupported_ShouldReturnFalse(string method)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.GetCanonicalizedValue(method);

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData("Get")]
    [InlineData("Head")]
    public void CanCache_WhenRequestContainsAuthorizationHeader_ShouldReturnFalse(string method)
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.GetCanonicalizedValue(method);
        context.Request.Headers[HeaderNames.Authorization] = "Bearer token";

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }
}