namespace Core.Cache.Storage.Abstractions;

internal interface IKeyBuilder
{
    string BuildCacheKey(string key);

    string BuildTag(string tag);

    string BuildTagsIndex(string key);

    string BuildLock(string key);
}