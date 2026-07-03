namespace Core.DistributedCache.Services.Rehydration;

internal interface ICacheRehydrator
{
    Task RehydrateAsync(CancellationToken cancellationToken);
}