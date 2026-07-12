using Core.Idempotency.Options;
using Core.Idempotency.Storage.Abstractions;

namespace Core.Idempotency.Storage.Redis;

internal sealed class RedisKeyBuilder(IdempotencyOptions options) : IKeyBuilder
{
    private readonly string _prefix = string.IsNullOrWhiteSpace(options.InstanceName)
        ? string.Empty
        : $"{options.InstanceName}:";

    public string BuildCacheKey(string key)
        => $"{_prefix}Idempotency:{key}";
}
