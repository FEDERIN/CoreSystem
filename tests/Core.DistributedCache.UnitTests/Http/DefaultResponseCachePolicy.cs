using Core.DistributedCache.Http.Caching;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Core.DistributedCache.UnitTests.Http;

public sealed class DefaultResponseCachePolicyTests
{
    private readonly DefaultResponseCachePolicy _policy = new();

    [Fact]
    public void CanCache_WhenStatusCodeIs200_ShouldReturnTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.StatusCode = StatusCodes.Status200OK;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanCache_WhenStatusCodeIs404_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCache_WhenStatusCodeIs500_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
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

    [Fact]
    public void CanCache_WhenCacheControlIsPrivate_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers.Append(
            HeaderNames.CacheControl,
            "private");

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCache_WhenCacheControlContainsPrivateDirective_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers.Append(
            HeaderNames.CacheControl,
            "public,max-age=60,private");

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCache_WhenCacheControlIsNoStore_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers.Append(
            HeaderNames.CacheControl,
            "no-store");

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCache_WhenCacheControlContainsNoStoreDirective_ShouldReturnFalse()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers.Append(
            HeaderNames.CacheControl,
            "public,max-age=120,no-store");

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCache_WhenCacheControlIsPublic_ShouldReturnTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.Headers.Append(
            HeaderNames.CacheControl,
            "public,max-age=60");

        // Act
        var result = _policy.CanCache(context);

        // Assert
        result.Should().BeTrue();
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