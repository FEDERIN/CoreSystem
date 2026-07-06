using Core.Cache.Abstractions;

namespace Core.Cache.Storage.Rehydration.Tracking;

internal interface IRehydrationTracker
{
    void Track(
        string key,
        CacheProviderType origin);

    void Untrack(string key);
}