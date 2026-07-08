using StackExchange.Redis;

namespace Core.Redis.Connection;

internal sealed class RedisConnectionFactory
    : IRedisConnectionFactory
{
    public IConnectionMultiplexer Create(
        Action<ConfigurationOptions>? configure)
    {
        var options = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            ConnectRetry = 1,
            ConnectTimeout = 500,
            AsyncTimeout = 500,
            SyncTimeout = 500,
            KeepAlive = 60,
            BacklogPolicy = BacklogPolicy.FailFast,
            ReconnectRetryPolicy = new ExponentialRetry(1000)
        };

        configure?.Invoke(options);

        if (options.EndPoints.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one Redis endpoint must be configured.");
        }

        return ConnectionMultiplexer.Connect(options);
    }
}