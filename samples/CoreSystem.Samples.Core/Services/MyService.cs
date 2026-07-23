using Core.Cache.Abstractions;

namespace CoreSystem.Samples.Core.Services;

public sealed class MyService(ICoreCache cache) : IMyService
{
    public async Task<string> GetDataAsync(string id)
    {
        string key = $"product_{id}";

        var result = await cache.GetOrAddAsync(
                    key,
                    async ct =>
                    {
                        await Task.Delay(500, ct);
                        return $"Datos reales para el ID: {id} obtenidos a las {DateTime.Now:HH:mm:ss}";
                    },
                    TimeSpan.FromMinutes(5)
                );

        return result ?? "Valor por defecto o vacío";
    }
}