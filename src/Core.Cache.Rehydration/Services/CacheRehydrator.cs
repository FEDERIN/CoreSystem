using Core.Cache.Rehydration.Abstractions;
using Microsoft.Extensions.Logging;

namespace Core.Cache.Rehydration.Services;

internal sealed class CacheRehydrator(
    IRehydrationSource source,
    IRehydrationTarget target,
    ILogger<CacheRehydrator> logger)
    : ICacheRehydrator
{
    private const int BatchSize = 100;

    public async Task RehydrateAsync(
        CancellationToken cancellationToken)
    {
        var entries = source
            .GetEntries()
            .ToList();

        foreach (var batch in entries.Chunk(BatchSize))
        {
            foreach (var entry in batch)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await target.StoreAsync(
                        entry,
                        cancellationToken);

                    await source.RemoveForRehydrationAsync(
                        entry.Key,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Unable to rehydrate cache key '{Key}'. It will be retried later.",
                        entry.Key);
                }
            }

            await Task.Delay(
                TimeSpan.FromMilliseconds(100),
                cancellationToken);
        }
    }
}