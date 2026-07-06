using StackExchange.Redis;

namespace Core.Redis.Synchronization;

internal sealed class RedisLock(
    IDatabase database,
    string key,
    string token)
    : IDisposable
{
    public void Dispose()
    {
        database.LockRelease(key, token);
    }
}