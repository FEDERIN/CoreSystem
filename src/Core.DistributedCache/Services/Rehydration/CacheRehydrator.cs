using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Services.Rehydration;
using Core.DistributedCache.Storage.Memory;
using Core.DistributedCache.Storage.Redis;
using Microsoft.Extensions.Logging;

internal sealed class CacheRehydrator(
        MemoryStorage memoryStorage,
        RedisStorage redisStorage,
        ILogger<CacheRehydrator> logger)
        : ICacheRehydrator
    {
        public async Task RehydrateAsync(CancellationToken ct)
        {
            const int BatchSize = 100;

            var keys = memoryStorage.GetTrackedKeys().ToList();

            foreach (var batch in keys.Chunk(BatchSize))
            {
                foreach (var key in batch)
                {
                    ct.ThrowIfCancellationRequested();

                    var wrapper =
                        memoryStorage.GetWrapper<object>(key);

                    if (wrapper is null)
                        continue;

                    if (wrapper.Origin != CacheProviderType.Redis)
                        continue;

                    try
                    {
                        await redisStorage.SetAsync(
                            key,
                            wrapper.Value,
                            ct: ct);

                        await memoryStorage.RemoveAsync(
                            key,
                            ct);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(
                            ex,
                            "Unable to rehydrate cache key '{Key}'. It will be retried later.",
                            key);
                    }
                }

                await Task.Delay(100, ct);
            }
        }
    }