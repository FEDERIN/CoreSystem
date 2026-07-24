using Core.Cache.Storage.Redis;

namespace Core.Cache.Storage.Abstractions.Redis;

internal interface IRedisTagIndex : ICacheTagIndex<RedisStorage>
{
    Task<IReadOnlyCollection<string>> GetKeysAsync(
    string tag,
    CancellationToken cancellationToken = default);

    Task<long> CountAsync(
        string tag,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string tag,
        CancellationToken cancellationToken = default);
}