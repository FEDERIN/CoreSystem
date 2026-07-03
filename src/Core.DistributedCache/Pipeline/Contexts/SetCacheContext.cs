namespace Core.DistributedCache.Pipeline.Contexts;

public sealed class SetCacheContext<T> : CacheContext
{
    public required T Value { get; init; }

    public TimeSpan? Expiration { get; init; }

    public string[]? Tags { get; init; }

    public override Task ExecuteAsync()
    {
        return Storage.SetAsync(
            Key,
            Value,
            Expiration,
            Tags,
            CancellationToken);
    }
}