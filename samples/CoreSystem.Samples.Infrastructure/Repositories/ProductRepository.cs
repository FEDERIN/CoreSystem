using CoreSystem.Samples.Core.Interfaces;

namespace CoreSystem.Samples.Infrastructure.Repositories;

internal sealed class ProductRepository
    : IProductRepository
{
    public async Task<string?> GetByIdAsync(
        string id,
        CancellationToken ct = default)
    {
        await Task.Delay(500, ct);

        return $"Datos reales para el ID: {id} " +
               $"obtenidos a las {DateTime.Now:HH:mm:ss}";
    }
}