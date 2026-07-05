namespace Core.Cache.Storage.Rehydration;

internal interface IRehydrationTarget
{
    Task StoreAsync(
        CacheRehydrationEntry entry,
        CancellationToken ct = default);
}