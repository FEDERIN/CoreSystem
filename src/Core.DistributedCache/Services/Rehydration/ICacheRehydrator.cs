namespace Core.Cache.Services.Rehydration;

internal interface ICacheRehydrator
{
    Task RehydrateAsync(CancellationToken cancellationToken);
}