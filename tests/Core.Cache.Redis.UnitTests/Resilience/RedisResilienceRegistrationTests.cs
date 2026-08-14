using Core.Cache.Redis.Resilience;
using Core.Resilience.Abstractions;
using Core.Resilience.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.Redis.UnitTests.Resilience;

public sealed class RedisResilienceRegistrationTests
{
    [Fact]
    public void TryAddRedisCacheResilience_ReturnsFalse_WhenResilienceOptionsAreNotRegistered()
    {
        var services = new ServiceCollection();

        var result =
            services.TryAddRedisCacheResilience();

        Assert.False(result);
    }

    [Fact]
    public void TryAddRedisCacheResilience_ReturnsFalse_WhenRedisPipelineIsNotConfigured()
    {
        var services = new ServiceCollection();

        var options = new ResilienceOptions();

        services.AddSingleton(options);

        var result =
            services.TryAddRedisCacheResilience();

        Assert.False(result);
    }

    [Fact]
    public void TryAddRedisCacheResilience_ReturnsTrue_WhenRedisPipelineIsConfigured()
    {
        var services = new ServiceCollection();

        var options = new ResilienceOptions();

        options.AddPipeline(
            PipelineType.Redis,
            _ => { });

        services.AddSingleton(options);

        var result =
            services.TryAddRedisCacheResilience();

        Assert.True(result);
    }

}