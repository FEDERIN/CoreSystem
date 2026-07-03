using Core.Cache.Storage.Abstractions;
using Core.Cache.Storage.Memory;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using StackExchange.Redis;


namespace Core.Cache.DependencyInjection;

internal static class ResilienceRegistration
{
    public static IServiceCollection AddCacheResilience(
    this IServiceCollection services)
    {
        services.AddSingleton(sp =>
            new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(200),
                    BackoffType = DelayBackoffType.Exponential,
                    ShouldHandle = new PredicateBuilder()
                        .Handle<RedisConnectionException>()
                        .Handle<RedisTimeoutException>()
                        .Handle<TimeoutException>()
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    MinimumThroughput = 10,
                    BreakDuration = TimeSpan.FromSeconds(15),
                    ShouldHandle = new PredicateBuilder()
                        .Handle<RedisConnectionException>()
                        .Handle<RedisTimeoutException>()
                        .Handle<TimeoutException>()
                })
                .Build());

        return services;
    }
}
