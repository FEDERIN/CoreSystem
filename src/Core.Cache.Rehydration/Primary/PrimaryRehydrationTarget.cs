using Core.Cache.Abstractions;
using Core.Cache.Rehydration.Abstractions;

namespace Core.Cache.Rehydration.Primary;

internal sealed class PrimaryRehydrationTarget(
    ICacheStorageResolver resolver)
    : IRehydrationTarget
{
    public Task StoreAsync(
        CacheRehydrationEntry entry,
        CancellationToken ct = default)
    {
        return resolver.Primary.SetAsync(
            entry.Key,
            entry.Value,
            expiration: entry.RemainingExpiration,
            tags: entry.Tags?.ToArray(),
            ct: ct);
    }
}