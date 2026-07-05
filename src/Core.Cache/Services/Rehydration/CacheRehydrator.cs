using Core.Cache.Abstractions;
using Core.Cache.Services.Rehydration;
using Core.Cache.Storage.Rehydration;
using Microsoft.Extensions.Logging;

internal sealed class CacheRehydrator(
    IRehydrationSource source,
    IRehydrationTarget target,
        ILogger<CacheRehydrator> logger)
        : ICacheRehydrator
    {
    public async Task RehydrateAsync(CancellationToken ct)
    {
        const int BatchSize = 100;

        var entries = source.GetEntries().ToList();

        foreach (var batch in entries.Chunk(BatchSize))
        {
            foreach (var entry in batch)
            {
                ct.ThrowIfCancellationRequested();

                if (entry.Origin != CacheProviderType.Redis)
                    continue;

                try
                    {
                        await target.StoreAsync(
                            entry,
                            ct);

                        await source.RemoveForRehydrationAsync(
                            entry.Key,
                            ct);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(
                            ex,
                            "Unable to rehydrate cache key '{Key}'. It will be retried later.",
                            entry.Key);
                    }
                }

            await Task.Delay(100, ct);
        }
    }
}