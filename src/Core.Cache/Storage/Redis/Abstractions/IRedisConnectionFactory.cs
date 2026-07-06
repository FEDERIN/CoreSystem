using Core.Cache.Options;
using StackExchange.Redis;

namespace Core.Cache.Storage.Redis.Abstractions;

internal interface IRedisConnectionFactory
{
    IConnectionMultiplexer Create(CacheOptions options);
}