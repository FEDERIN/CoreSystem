using Core.Cache.Abstractions;
using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Storage.Rehydration.Tracking;

internal sealed class RehydrationTracker(
    ICacheKeyTracker tracker)
    : IRehydrationTracker
{
    public void Track(
        string key,
        CacheProviderType origin)
    {
        if (origin != CacheProviderType.Redis)
            return;

        tracker.Track(key);
    }

    public void Untrack(string key)
    {
        tracker.Untrack(key);
    }
}