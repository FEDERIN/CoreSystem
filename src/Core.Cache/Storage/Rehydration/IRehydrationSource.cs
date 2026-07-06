namespace Core.Cache.Storage.Rehydration;

internal interface IRehydrationSource
{
    IEnumerable<CacheRehydrationEntry> GetEntries();

    Task RemoveForRehydrationAsync(
        string key,
        CancellationToken ct = default);
}