using StackExchange.Redis;

namespace Core.Redis.Connection;

public interface IRedisConnectionFactory
{
    IConnectionMultiplexer Create(
        Action<ConfigurationOptions>? configure);
}