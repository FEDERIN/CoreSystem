using Core.DistributedCache.Abstractions;
using Core.DistributedCache.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.UnitTests;

public abstract class MemoryCacheTestBase
{
    protected static ICoreCacheService CreateCache()
    {
        var services = new ServiceCollection();

        services.AddCoreDistributedCache(options =>
        {
            options.DefaultProvider = CacheProviderType.Memory;
        });

        var provider = services.BuildServiceProvider();

        return provider.GetRequiredService<ICoreCacheService>();
    }
}