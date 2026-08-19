using Core.Cache.Abstractions;
using Core.Cache.Storage.Abstractions;
using Core.Cache.Storage.Memory;

namespace Core.Cache.Storage;

internal sealed class CacheStorageResolver : ICacheStorageResolver
{
    public CacheStorageResolver(
        MemoryStorage memoryStorage,
        IEnumerable<IExternalCacheStorage> externalStorages)
    {
        ArgumentNullException.ThrowIfNull(memoryStorage);
        ArgumentNullException.ThrowIfNull(externalStorages);

        var externalStorage = externalStorages.ToList();

        if (externalStorage.Count > 1)
        {
            throw new InvalidOperationException(
                "Only one external cache storage can be registered as the primary cache storage.");
        }

        if (externalStorage.Count == 0)
        {
            Primary = memoryStorage;
            Fallback = null;
            HasFallback = false;

            return;
        }

        Primary = externalStorage[0];
        Fallback = memoryStorage;
        HasFallback = true;
    }

    public ICacheStorage Primary { get; }

    public ICacheStorage? Fallback { get; }

    public bool HasFallback { get; }
}