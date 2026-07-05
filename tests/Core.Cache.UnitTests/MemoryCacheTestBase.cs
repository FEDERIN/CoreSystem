using Core.Cache.Abstractions;
using Core.Cache.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.UnitTests;

public abstract class MemoryCacheTestBase
{
    protected static ICoreCache CreateCache()
    {
        var services = new ServiceCollection();

        services.AddCoreCache(options =>
        {
            options.DefaultProvider = CacheProviderType.Memory;
        });

        var provider = services.BuildServiceProvider();

        return provider.GetRequiredService<ICoreCache>();
    }
}