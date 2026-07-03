using Core.Cache.Abstractions;
using Core.Cache.DependencyInjection;
using Core.Cache.Options;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.UnitTests.DependencyInjection;

public class CacheRegistrationTests
{
    [Fact]
    public void AddCoreDistributedCache_RegistersAllRequiredServicesPerProviderMemory()
    {
        var services = new ServiceCollection();

        services.AddCoreDistributedCache(options => {
            options.DefaultProvider = CacheProviderType.Memory;
        });

        var provider = services.BuildServiceProvider();

        provider.GetService<ICoreCacheService>().Should().NotBeNull();
        provider.GetService<ICacheStorageResolver>().Should().NotBeNull();
        provider.GetService<CacheOptions>().Should().NotBeNull();
    }

    [Fact]
    public void AddCoreDistributedCache_RegistersAllRequiredServicesPerProviderRedis()
    {
        var services = new ServiceCollection();

        services.AddCoreDistributedCache(options => {
            options.DefaultProvider = CacheProviderType.Redis;
            options.Redis = new RedisOptions
            {
                Enabled = true,
                Configuration = config =>
                {
                    config.EndPoints.Add("localhost:6379");
                    config.Password = "foobar";
                }
            };
        });

        var provider = services.BuildServiceProvider();

        provider.GetService<ICoreCacheService>().Should().NotBeNull();
        provider.GetService<ICacheStorageResolver>().Should().NotBeNull();
        provider.GetService<CacheOptions>().Should().NotBeNull();
    }
}