namespace Core.DistributedCache.Services.Rehydration;

public interface IRedisRehydrationService
{
    Task ExecuteCycleAsync(CancellationToken cancellationToken);
}