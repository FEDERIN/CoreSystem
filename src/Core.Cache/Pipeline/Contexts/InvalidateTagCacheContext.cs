namespace Core.Cache.Pipeline.Contexts;

public sealed class InvalidateTagCacheContext : CacheContext
{
    public required string Tag { get; init; }

    public override Task ExecuteAsync()
    {
        return Storage.InvalidateByTagAsync(
            Tag,
            CancellationToken);
    }
}
