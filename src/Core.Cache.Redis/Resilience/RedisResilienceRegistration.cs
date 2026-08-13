using Core.Resilience.Abstractions;
using Core.Resilience.Options;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Cache.Redis.Resilience;

internal static class RedisResilienceRegistration
{
    internal static bool TryAddRedisCacheResilience(
        this IServiceCollection services)
    {
        var options = services
            .Where(x => x.ServiceType == typeof(ResilienceOptions))
            .Select(x => x.ImplementationInstance)
            .OfType<ResilienceOptions>()
            .FirstOrDefault();

        if (options is null)
            return false;

        if (!options.ContainsPipeline(PipelineType.Redis))
            return false;

        ApplyRedisExceptions(
            options.GetPipeline(PipelineType.Redis));

        return true;
    }

    private static void ApplyRedisExceptions(
        PipelineOptions pipeline)
    {
        pipeline.Retry?
            .Handle<RedisConnectionException>()
            .Handle<RedisTimeoutException>()
            .Handle<TimeoutException>();

        pipeline.CircuitBreaker?
            .Handle<RedisConnectionException>()
            .Handle<RedisTimeoutException>()
            .Handle<TimeoutException>();
    }
}