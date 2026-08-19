using Core.Cache.Abstractions;
using CoreSystem.Samples.Core.Interfaces;

namespace CoreSystem.Samples.Infrastructure.Repositories;

internal sealed class ProductRepository(
    ICoreCache cache)
    : IProductRepository
{
    public Task<string?> GetByIdAsync(
        string id,
        CancellationToken ct = default)
    {
        var key = $"product_{id}";

        return cache.GetOrAddAsync(
            key,
            async cancellationToken =>
            {
                await Task.Delay(500, cancellationToken);

                return $"Datos reales para el ID: {id} " +
                       $"obtenidos a las {DateTime.Now:HH:mm:ss}";
            },
            TimeSpan.FromMinutes(5),
            tags: ["data"],
            ct: ct);
    }
}