using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Options;
using Core.DistributedCache.Storage.Memory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.Tests;


public class DistributedCacheExtensionsTests
{
    [Fact]
    public void AddCoreDistributedCache_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreDistributedCache(options =>
        {
            options.DefaultExpiration = TimeSpan.FromMinutes(10);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<ICacheSerializer>());
        Assert.NotNull(serviceProvider.GetService<ICoreCacheService>());
        Assert.NotNull(serviceProvider.GetService<ICacheServiceFactory>());
        Assert.IsType<CacheOptions>(serviceProvider.GetService<CacheOptions>());
    }

    [Fact]
    public void AddCoreDistributedCache_WhenRedisDisabled_RegistersMemoryStorage()
    {
        var services = new ServiceCollection();
        services.AddCoreDistributedCache(options => {
            options.Redis.Enabled = false;
        });

        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ICoreCacheService>().Should().BeOfType<MemoryCacheStorage>();
    }
}