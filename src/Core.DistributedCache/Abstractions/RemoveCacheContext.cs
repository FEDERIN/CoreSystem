namespace Core.DistributedCache.Abstractions;

public sealed class RemoveCacheContext : CacheContext
{
    public override Task ExecuteAsync()
    {
        return Storage.RemoveAsync(
            Key,
            CancellationToken);
    }
}
