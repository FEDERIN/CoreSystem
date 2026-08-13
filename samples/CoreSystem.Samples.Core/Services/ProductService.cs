using CoreSystem.Samples.Core.Interfaces;

namespace CoreSystem.Samples.Core.Services;

public sealed class ProductService(
    IProductRepository repository)
    : IProductService
{
    public async Task<string> GetDataAsync(
        string id,
        CancellationToken ct = default)
    {
        return await repository.GetByIdAsync(id, ct)
            ?? "Producto no encontrado";
    }
}