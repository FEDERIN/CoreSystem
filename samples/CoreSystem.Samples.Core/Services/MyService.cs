using Core.DistributedCache.Abstractions;

namespace CoreSystem.Samples.Core.Services;

public class MyService(ICoreCacheService cache) : IMyService
{
    public async Task<string> GetDataAsync(string id)
    {
        string key = $"product_{id}";

        var resultado = await cache.GetOrAddAsync(key, async () =>
        {
            await Task.Delay(500);
            return $"Datos reales para el ID: {id} obtenidos a las {DateTime.Now:HH:mm:ss}";
        }, TimeSpan.FromMinutes(5));

        return resultado ?? "Valor por defecto o vacío";
    }
}