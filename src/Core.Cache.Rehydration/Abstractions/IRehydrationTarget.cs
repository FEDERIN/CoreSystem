namespace Core.Cache.Rehydration.Abstractions;

internal interface IRehydrationTarget
{
    Task StoreAsync(
        CacheRehydrationEntry entry,
        CancellationToken ct = default);
}