using Core.Cache.Abstractions;

namespace Core.Cache.Storage.Rehydration;

internal sealed record CacheRehydrationEntry
{
    public required string Key { get; init; }

    public required object Value { get; init; }

    public required CacheProviderType Origin { get; init; }
}