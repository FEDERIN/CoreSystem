namespace Core.DistributedCache.Abstractions;

public abstract class CacheContext
{
    public required string Key { get; init; }

    public ICacheStorage Storage { get; set; } = default!;

    public CancellationToken CancellationToken { get; init; }

    public Exception? Exception { get; set; }
    public abstract Task ExecuteAsync();
}
