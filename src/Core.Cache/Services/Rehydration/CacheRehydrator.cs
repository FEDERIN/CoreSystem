using Core.Cache.Abstractions;
using Core.Cache.Services.Rehydration;
using Core.Cache.Storage.Memory;
using Core.Cache.Storage.Redis;
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

                var entry = memoryStorage.GetEntry(key);

                if (entry is null)
                    continue;

                if (!memoryStorage.TryGetOrigin(entry, out var origin))
                    continue;

                if (origin != CacheProviderType.Redis)
                    continue;

                if (!memoryStorage.TryGetValue(entry, out var value))
                    continue;

                try
                {
                    await redisStorage.SetAsync(
                        key,
                        value,
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