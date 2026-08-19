namespace Core.Cache.Redis.DependencyInjection;

internal static class RedisMessages
{
    public const string RedisConfigurationRequired =
        "Redis configuration is required when using the Redis cache provider.";

    public const string CacheRegistrationRequired =
    "Cache options are required when using the Redis cache provider.";
}
