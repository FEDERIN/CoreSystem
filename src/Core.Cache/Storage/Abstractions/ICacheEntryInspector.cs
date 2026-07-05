namespace Core.Cache.Storage.Abstractions;

internal interface ICacheEntryInspector
{
    bool TryGet(
    object? entry,
    out ICacheEntry? wrapper);

    bool TryGetValue<T>(
        object? entry,
        out T? value);
}