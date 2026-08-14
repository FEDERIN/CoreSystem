namespace Core.Cache.Rehydration.Abstractions;

internal interface IRehydrationSource
{
    IEnumerable<CacheRehydrationEntry> GetEntries();

    Task RemoveForRehydrationAsync(
        string key,
        CancellationToken ct = default);
}