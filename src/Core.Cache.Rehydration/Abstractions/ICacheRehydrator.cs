namespace Core.Cache.Rehydration.Abstractions;

internal interface ICacheRehydrator
{
    Task RehydrateAsync(CancellationToken cancellationToken);
}