using Core.Cache.Storage.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Cache.Storage.Rehydration;

internal sealed class MemoryRehydrationSource(
    IMemoryCache memoryCache,
    ICacheKeyTracker tracker,
    ICacheEntryFactory entryFactory)
    : IRehydrationSource
{
    public IEnumerable<CacheRehydrationEntry> GetEntries()
    {
        foreach (var key in tracker.GetAllTrackedKeys())
        {
            if (!memoryCache.TryGetValue(key, out object? entry))
                continue;

            if (!entryFactory.TryGetOrigin(entry, out var origin))
                continue;

            if (!entryFactory.TryGetValue(entry, out var value))
                continue;

            yield return new CacheRehydrationEntry
            {
                Key = key,
                Origin = origin,
                Value = value!
            };
        }
    }

    public Task RemoveForRehydrationAsync(
        string key,
        CancellationToken ct = default)
    {
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}