namespace Core.Cache.Rehydration.Abstractions;

internal sealed record CacheRehydrationEntry
{
    public required string Key { get; init; }

    public required object Value { get; init; }

    public TimeSpan? RemainingExpiration { get; init; }

    public IReadOnlyCollection<string>? Tags { get; init; }
}
