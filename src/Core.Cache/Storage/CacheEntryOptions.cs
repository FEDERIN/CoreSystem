namespace Core.Cache.Storage;

public sealed class CacheEntryOptions
{
    public static readonly CacheEntryOptions Default = new();

    public static readonly CacheEntryOptions Rehydrate =
        new()
        {
            TrackForRehydration = true
        };

    public bool TrackForRehydration { get; init; }
}
