namespace Core.Cache.Storage.Abstractions;

internal interface ICacheEntry
{
    object Value { get; }
    DateTimeOffset? AbsoluteExpiration { get; }
    IReadOnlyCollection<string>? Tags { get; }
}