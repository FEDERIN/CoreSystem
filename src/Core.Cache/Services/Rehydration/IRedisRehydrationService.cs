namespace Core.Cache.Services.Rehydration;

public interface IRedisRehydrationService
{
    Task ExecuteCycleAsync(CancellationToken cancellationToken);
}