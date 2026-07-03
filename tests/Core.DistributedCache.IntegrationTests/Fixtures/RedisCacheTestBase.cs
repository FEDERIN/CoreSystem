using Core.DistributedCache.Abstractions;
using Core.DistributedCache.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.DistributedCache.IntegrationTests.Fixtures;

public abstract class RedisCacheTestBase
{
    protected IServiceProvider Services { get; }

    protected ICoreCacheService Cache { get; }

    protected IConnectionMultiplexer Connection { get; }

    protected IDatabase Database => Connection.GetDatabase();

    protected string InstanceName { get; private set; } = string.Empty;

    protected string BuildRedisKey(string key)
        => string.IsNullOrWhiteSpace(InstanceName)
            ? key
            : $"{InstanceName}:{key}";

    protected RedisCacheTestBase(RedisContainerFixture fixture)
    {
        Connection = ConnectionMultiplexer.Connect(fixture.ConnectionString);

        var services = new ServiceCollection();

        services.AddCoreDistributedCache(options =>
        {
            options.DefaultProvider = CacheProviderType.Redis;

            // Si luego decides usar un prefijo, solo cambia esta línea.
            // options.InstanceName = "CoreSystem";
            InstanceName = options.InstanceName ?? string.Empty;

            options.Redis.Configuration = config =>
            {
                config.EndPoints.Add(fixture.ConnectionString);
            };
        });

        Services = services.BuildServiceProvider();

        Cache = Services.GetRequiredService<ICoreCacheService>();
    }
}