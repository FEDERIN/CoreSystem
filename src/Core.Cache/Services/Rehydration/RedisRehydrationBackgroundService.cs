using Core.Cache.Options;
using Microsoft.Extensions.Hosting;

namespace Core.Cache.Services.Rehydration;

internal sealed class RedisRehydrationBackgroundService(
    IRedisRehydrationService service,
    CacheOptions options)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await service.ExecuteCycleAsync(
                stoppingToken);

            await Task.Delay(options.RehydrationInterval,
                stoppingToken);
        }
    }
}