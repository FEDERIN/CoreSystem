using Core.Cache.Abstractions;
using Core.Cache.DependencyInjection;
using Core.Cache.Options;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.UnitTests.DependencyInjection;

public class CacheRegistrationTests
{
    [Fact]
    public void AddCoreCache_RegistersAllRequiredServices()
    {
        var services = new ServiceCollection();

        services.AddCoreCache(options => {});

        var provider = services.BuildServiceProvider();

        provider.GetService<ICoreCache>().Should().NotBeNull();
        provider.GetService<ICacheStorageResolver>().Should().NotBeNull();
        provider.GetService<CacheOptions>().Should().NotBeNull();
    }

    [Fact]
    public void AddCoreCache_Should_ReturnServices_WhenCacheIsDisabled()
    {
        var services = new ServiceCollection();

        var result = services.AddCoreCache(options =>
        {
            options.Enabled = false;
        });

        Assert.Same(services, result);
    }
}