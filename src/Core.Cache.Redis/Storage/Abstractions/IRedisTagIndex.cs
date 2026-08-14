using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Redis.Storage.Abstractions;

internal interface IRedisTagIndex : ICacheTagIndex<RedisCacheStorage>
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