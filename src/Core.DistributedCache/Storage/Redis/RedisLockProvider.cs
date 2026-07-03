using Core.DistributedCache.Storage.Abstractions;
using StackExchange.Redis;

namespace Core.DistributedCache.Storage.Redis;

internal sealed class RedisLockProvider(
    IConnectionMultiplexer connection,
    IKeyBuilder keyBuilder)
    : ICacheLockProvider<RedisStorage>
{
    private readonly IDatabase _database = connection.GetDatabase();

    public async Task<IDisposable> AcquireAsync(
        string key,
        CancellationToken ct = default)
    {
        var lockKey = keyBuilder.BuildLock(key);
        var token = Guid.NewGuid().ToString();

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            if (await _database.LockTakeAsync(
                    lockKey,
                    token,
                    TimeSpan.FromSeconds(30)))
            {
                return new RedisLock(
                    _database,
                    lockKey,
                    token);
            }

            await Task.Delay(100, ct);
        }
    }

    private sealed class RedisLock(
        IDatabase database,
        RedisKey key,
        RedisValue token)
        : IDisposable
    {
        public void Dispose()
        {
            database.LockRelease(key, token);
        }
    }
}