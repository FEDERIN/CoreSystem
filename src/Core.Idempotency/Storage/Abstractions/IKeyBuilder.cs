namespace Core.Idempotency.Storage.Abstractions;

internal interface IKeyBuilder
{
    string BuildCacheKey(string key);
    string BuildLock(string key);
}