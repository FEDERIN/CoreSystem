using Core.Redis.Options;
using StackExchange.Redis;
using System.Diagnostics;

namespace Core.Redis.Synchronization;

internal sealed class RedisLockProvider(
    IConnectionMultiplexer connectionMultiplexer,
    RedisLockOptions options)
    : IDistributedLockProvider
{
    private readonly IDatabase _database =
        connectionMultiplexer.GetDatabase();

    public async Task<IDisposable> AcquireAsync(
        string lockKey,
        CancellationToken ct = default)
    {
        var token = Guid.NewGuid().ToString("N");
        var started = Stopwatch.GetTimestamp();

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            if (await _database.LockTakeAsync(
                    lockKey,
                    token,
                    options.LockDuration))
            {
                return new RedisLock(
                    _database,
                    lockKey,
                    token);
            }

            if (options.MaxWaitTime.HasValue &&
                Stopwatch.GetElapsedTime(started) >= options.MaxWaitTime.Value)
            {
                throw new TimeoutException(
                    $"Unable to acquire Redis lock '{lockKey}'.");
            }

            await Task.Delay(options.RetryDelay, ct);
        }
    }
}