namespace Core.DistributedCache.Abstractions;

public sealed class ExistsCacheContext : CacheContext
{
    public bool Exists { get; set; }

    public override async Task ExecuteAsync()
    {
        Exists = await Storage.ExistsAsync(
            Key,
            CancellationToken);
    }
}