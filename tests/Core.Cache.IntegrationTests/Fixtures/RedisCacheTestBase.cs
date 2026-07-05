using Core.Cache.Abstractions;
using Core.Cache.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Cache.IntegrationTests.Fixtures;

public abstract class RedisCacheTestBase
{
    protected IServiceProvider Services { get; }

    protected ICoreCache Cache { get; }

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

        services.AddCoreCache(options =>
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

        Cache = Services.GetRequiredService<ICoreCache>();
    }
}