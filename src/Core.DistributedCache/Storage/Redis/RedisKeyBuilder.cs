using Core.Cache.Options;
using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Storage.Redis;

internal sealed class RedisKeyBuilder(CacheOptions options) : IKeyBuilder
{
    private readonly string _prefix = string.IsNullOrWhiteSpace(options.InstanceName)
            ? string.Empty
            : $"{options.InstanceName}:";

    public string BuildCacheKey(string key)
        => $"{_prefix}{key}";

    public string BuildTag(string tag)
        => $"{_prefix}tag:{tag}";

    public string BuildLock(string key)
        => $"{BuildCacheKey(key)}:lock";
    public string BuildTagsIndex(string key)
    => $"{BuildCacheKey(key)}:tags";
}