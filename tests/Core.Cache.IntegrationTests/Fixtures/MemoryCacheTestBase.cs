using Core.Cache.Abstractions;
using Core.Cache.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.IntegrationTests.Fixtures;

public abstract class MemoryCacheTestBase
{
    protected IServiceProvider Services { get; }

    protected ICoreCache Cache { get; }

    protected MemoryCacheTestBase()
    {
        var services = new ServiceCollection();

        services.AddCoreCache(options =>
        {
            options.DefaultProvider = CacheProviderType.Memory;
        });

        Services = services.BuildServiceProvider();

        Cache = Services.GetRequiredService<ICoreCache>();
    }
}