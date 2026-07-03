using Core.Cache.Abstractions;
using Core.Cache.Storage;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Core.Cache.UnitTests.Storage;

public sealed class CacheServiceFactoryTests
{
    [Fact]
    public void GetDefaultCache_ShouldReturnRegisteredCacheService()
    {
        // Arrange
        var cache = new Mock<ICoreCacheService>();

        var services = new ServiceCollection();
        services.AddSingleton(cache.Object);

        var provider = services.BuildServiceProvider();

        var resolver = new Mock<ICacheStorageResolver>();

        var sut = new CacheServiceFactory(
            provider,
            resolver.Object);

        // Act
        var result = sut.GetDefaultCache();

        // Assert
        result.Should().BeSameAs(cache.Object);
    }

    [Fact]
    public void GetStorage_WhenProviderIsRedis_ShouldReturnPrimaryStorage()
    {
        // Arrange
        var primary = new Mock<ICacheStorage>();
        var fallback = new Mock<ICacheStorage>();

        var resolver = new Mock<ICacheStorageResolver>();

        resolver
            .Setup(x => x.Primary)
            .Returns(primary.Object);

        resolver
            .Setup(x => x.Fallback)
            .Returns(fallback.Object);

        var sut = new CacheServiceFactory(
            Mock.Of<IServiceProvider>(),
            resolver.Object);

        // Act
        var result = sut.GetStorage(CacheProviderType.Redis);

        // Assert
        result.Should().BeSameAs(primary.Object);
    }

    [Fact]
    public void GetStorage_WhenProviderIsMemory_ShouldReturnFallbackStorage()
    {
        // Arrange
        var primary = new Mock<ICacheStorage>();
        var fallback = new Mock<ICacheStorage>();

        var resolver = new Mock<ICacheStorageResolver>();

        resolver
            .Setup(x => x.Primary)
            .Returns(primary.Object);

        resolver
            .Setup(x => x.Fallback)
            .Returns(fallback.Object);

        var sut = new CacheServiceFactory(
            Mock.Of<IServiceProvider>(),
            resolver.Object);

        // Act
        var result = sut.GetStorage(CacheProviderType.Memory);

        // Assert
        result.Should().BeSameAs(fallback.Object);
    }

    [Fact]
    public void GetStorage_WhenProviderIsUnknown_ShouldThrowNotSupportedException()
    {
        // Arrange
        var resolver = new Mock<ICacheStorageResolver>();

        var sut = new CacheServiceFactory(
            Mock.Of<IServiceProvider>(),
            resolver.Object);

        // Act
        Action action = () =>
            sut.GetStorage((CacheProviderType)999);

        // Assert
        action.Should()
            .Throw<NotSupportedException>();
    }
}