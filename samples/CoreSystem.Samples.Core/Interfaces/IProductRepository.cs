namespace CoreSystem.Samples.Core.Interfaces;

public interface IProductRepository
{
    Task<string?> GetByIdAsync(
        string id,
        CancellationToken ct = default);
}