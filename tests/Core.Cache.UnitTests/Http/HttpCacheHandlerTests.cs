using Core.Cache.Abstractions;
using Core.Cache.Http;
using Core.Cache.Options;
using Core.Cache.Attributes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Core.Http.Abstractions;
using Core.Http.Responses;

namespace Core.Cache.UnitTests.Http;

public sealed class HttpCacheHandlerTests
{
    private readonly Mock<ICoreCache> _cache = new();
    private readonly Mock<IRequestCachePolicy> _requestPolicy = new();
    private readonly Mock<IResponseCachePolicy> _responsePolicy = new();
    private readonly Mock<ICacheKeyGenerator> _keyGenerator = new();
    private readonly Mock<IResponseCapture> _responseCapture = new();
    private readonly Mock<IHttpResponseWriter> _responseWriter = new();

    private readonly CacheOptions _options = new()
    {
        DefaultExpiration = TimeSpan.FromMinutes(30)
    };

    [Fact]
    public async Task HandleAsync_WhenEndpointHasNoCacheableAttribute_ShouldInvokeNext()
    {
        // Arrange
        var handler = CreateHandler();

        var context = CreateHttpContext(cacheable: false);

        var nextCalled = false;

        Task Next(HttpContext _)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }

        // Act
        await handler.HandleAsync(context, Next);

        // Assert
        nextCalled.Should().BeTrue();

        _cache.Verify(
            x => x.GetAsync<CapturedResponse>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenRequestPolicyReturnsFalse_ShouldInvokeNext()
    {
        // Arrange
        var handler = CreateHandler();

        var context = CreateHttpContext();

        _requestPolicy
            .Setup(x => x.CanCache(context))
            .Returns(false);

        var nextCalled = false;

        Task Next(HttpContext _)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }

        // Act
        await handler.HandleAsync(context, Next);

        // Assert
        nextCalled.Should().BeTrue();

        _cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WhenCacheHit_ShouldWriteCachedResponse()
    {
        // Arrange
        var handler = CreateHandler();

        var context = CreateHttpContext();

        _requestPolicy
            .Setup(x => x.CanCache(context))
            .Returns(true);

        _keyGenerator
            .Setup(x => x.Generate(context))
            .Returns("customer:1");

        var response = new CapturedResponse
        {
            Body = [],
            StatusCode = 200,
            Headers = new Dictionary<string, string[]>()
        };

        _cache
            .Setup(x => x.GetAsync<CapturedResponse>(
                "customer:1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        await handler.HandleAsync(
            context,
            _ => Task.CompletedTask);

        // Assert

        _responseWriter.Verify(
            x => x.WriteAsync(context, response),
            Times.Once);

        _responseCapture.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_WhenCacheMiss_ShouldCaptureResponse()
    {
        // Arrange

        var handler = CreateHandler();

        var context = CreateHttpContext();

        _requestPolicy
            .Setup(x => x.CanCache(context))
            .Returns(true);

        _responsePolicy
            .Setup(x => x.CanCache(context))
            .Returns(false);

        _keyGenerator
            .Setup(x => x.Generate(context))
            .Returns("customer:1");

        _cache
            .Setup(x => x.GetAsync<CapturedResponse>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CapturedResponse?)null);

        _responseCapture
            .Setup(x => x.CaptureAsync(
                context,
                It.IsAny<RequestDelegate>()))
            .ReturnsAsync(new CapturedResponse
            {
                Body = [],
                StatusCode = 200,
                Headers = new Dictionary<string, string[]>()
            });

        // Act

        await handler.HandleAsync(
            context,
            _ => Task.CompletedTask);

        // Assert

        _responseCapture.Verify(
            x => x.CaptureAsync(
                context,
                It.IsAny<RequestDelegate>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenResponseCannotBeCached_ShouldNotStoreResponse()
    {
        // Arrange

        var handler = CreateHandler();

        var context = CreateHttpContext();

        _requestPolicy
            .Setup(x => x.CanCache(context))
            .Returns(true);

        _responsePolicy
            .Setup(x => x.CanCache(context))
            .Returns(false);

        _keyGenerator
            .Setup(x => x.Generate(context))
            .Returns("customer:1");

        _cache
            .Setup(x => x.GetAsync<CapturedResponse>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CapturedResponse?)null);

        _responseCapture
            .Setup(x => x.CaptureAsync(
                context,
                It.IsAny<RequestDelegate>()))
            .ReturnsAsync(new CapturedResponse
            {
                Body = [],
                StatusCode = 200,
                Headers = new Dictionary<string, string[]>()
            });

        // Act

        await handler.HandleAsync(
            context,
            _ => Task.CompletedTask);

        // Assert

        _cache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<CapturedResponse>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<string[]?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenResponseCanBeCached_ShouldStoreResponse()
    {
        // Arrange

        var handler = CreateHandler();

        var context = CreateHttpContext();

        _requestPolicy.Setup(x => x.CanCache(context)).Returns(true);

        _responsePolicy.Setup(x => x.CanCache(context)).Returns(true);

        _keyGenerator.Setup(x => x.Generate(context)).Returns("customer:1");

        _cache
            .Setup(x => x.GetAsync<CapturedResponse>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((CapturedResponse?)null);

        _responseCapture
            .Setup(x => x.CaptureAsync(
                context,
                It.IsAny<RequestDelegate>()))
            .ReturnsAsync(new CapturedResponse
            {
                Body = [],
                StatusCode = 200,
                Headers = new Dictionary<string, string[]>()
            });

        // Act

        await handler.HandleAsync(
            context,
            _ => Task.CompletedTask);

        // Assert

        _cache.Verify(
            x => x.SetAsync(
                "customer:1",
                It.IsAny<CapturedResponse>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<string[]?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private HttpCacheHandler CreateHandler()
    {
        return new HttpCacheHandler(
            _cache.Object,
            _options,
            _requestPolicy.Object,
            _responsePolicy.Object,
            _keyGenerator.Object,
            _responseCapture.Object,
            _responseWriter.Object);
    }

    private static DefaultHttpContext CreateHttpContext(
        bool cacheable = true)
    {
        var context = new DefaultHttpContext();

        var endpoint = cacheable
            ? new Endpoint(
                _ => Task.CompletedTask,
                new EndpointMetadataCollection(
                    new CacheableAttribute()),
                "test")
            : new Endpoint(
                _ => Task.CompletedTask,
                new EndpointMetadataCollection(),
                "test");

        context.SetEndpoint(endpoint);

        return context;
    }
}