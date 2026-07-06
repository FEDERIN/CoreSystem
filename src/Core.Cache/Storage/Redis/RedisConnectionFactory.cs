using Core.Cache.Options;
using Core.Cache.Storage.Redis.Abstractions;
using StackExchange.Redis;

namespace Core.Cache.Storage.Redis;

internal sealed class RedisConnectionFactory
    : IRedisConnectionFactory
{
    public IConnectionMultiplexer Create(CacheOptions options)
    {
        var config = new ConfigurationOptions();

        options.Redis.Configuration?.Invoke(config);

        config.AbortOnConnectFail = false;
        config.ConnectRetry = 1;
        config.ConnectTimeout = 500;
        config.AsyncTimeout = 500;
        config.SyncTimeout = 500;
        config.BacklogPolicy = BacklogPolicy.FailFast;
        config.ReconnectRetryPolicy = new ExponentialRetry(1000);
        config.KeepAlive = 60;

        return ConnectionMultiplexer.Connect(config);
    }
}