using Core.Cache.Storage.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Cache.Storage.Rehydration;

internal sealed class MemoryRehydrationSource(
    IMemoryCache memoryCache,
    ICacheKeyTracker tracker,
    ICacheEntryInspector entryInspector)
    : IRehydrationSource
{
    public IEnumerable<CacheRehydrationEntry> GetEntries()
    {
        foreach (var key in tracker.GetAllTrackedKeys())
        {
            if (!memoryCache.TryGetValue(key, out object? entry))
                continue;

            if (!entryInspector.TryGet(entry, out var wrapper))
                continue;

            if(wrapper == null)
                continue;

            TimeSpan? expiration = null;

            if (wrapper.AbsoluteExpiration.HasValue)
            {
                expiration = wrapper.AbsoluteExpiration.Value - DateTimeOffset.UtcNow;

                if (expiration <= TimeSpan.Zero)
                    continue;
            }

            yield return new CacheRehydrationEntry
            {
                Key = key,
                Origin = wrapper.Origin,
                Value = wrapper.Value,
                RemainingExpiration = expiration
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