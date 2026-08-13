namespace CoreSystem.Samples.Core.Interfaces;

public interface IProductService
{
    Task<string> GetDataAsync(
        string id,
        CancellationToken ct = default);
}