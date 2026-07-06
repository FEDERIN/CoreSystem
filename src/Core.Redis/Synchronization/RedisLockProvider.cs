using StackExchange.Redis;

namespace Core.Redis.Synchronization;

internal sealed class RedisLockProvider(
    IConnectionMultiplexer connectionMultiplexer)
    : IDistributedLockProvider
{
    private static readonly TimeSpan LockDuration = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(100);

    private readonly IDatabase _database =
        connectionMultiplexer.GetDatabase();

    public async Task<IDisposable> AcquireAsync(
        string lockKey,
        CancellationToken ct = default)
    {
        var token = Guid.NewGuid().ToString("N");

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            if (await _database.LockTakeAsync(
                    lockKey,
                    token,
                    LockDuration))
            {
                return new RedisLock(
                    _database,
                    lockKey,
                    token);
            }

            await Task.Delay(RetryDelay, ct);
        }
    }
}