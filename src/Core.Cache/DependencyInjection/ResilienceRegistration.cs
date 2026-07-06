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
                    MaxRetryAttempts = 1,
                    Delay = TimeSpan.FromMilliseconds(100),
                    BackoffType = DelayBackoffType.Constant,
                    ShouldHandle = new PredicateBuilder()
                        .Handle<RedisConnectionException>()
                        .Handle<RedisTimeoutException>()
                        .Handle<TimeoutException>()
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    MinimumThroughput = 2,
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
