namespace Core.Cache.Redis.Builders;

internal sealed class RedisKeyBuilder(string prefix) : IKeyBuilder
{
    public string BuildCacheKey(string key)
        => $"{prefix}{key}";

    public string BuildTag(string tag)
        => $"{prefix}tag:{tag}";

    public string BuildLock(string key)
        => $"{BuildCacheKey(key)}:lock";
    public string BuildTagsIndex(string key)
    => $"{BuildCacheKey(key)}:tags";
}