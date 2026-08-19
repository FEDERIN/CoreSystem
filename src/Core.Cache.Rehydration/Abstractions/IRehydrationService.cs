namespace Core.Cache.Rehydration.Abstractions;

internal interface IRehydrationService
{
    Task ExecuteCycleAsync(
        CancellationToken cancellationToken);
}