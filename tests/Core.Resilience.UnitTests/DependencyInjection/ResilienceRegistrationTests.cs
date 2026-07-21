using Core.Resilience.Abstractions;
using Core.Resilience.Builders.Abstractions;
using Core.Resilience.DependencyInjection;
using Core.Resilience.Internal;
using Core.Resilience.Internal.Builders;
using Core.Resilience.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Core.Resilience.UnitTests.DependencyInjection;

public sealed class ResilienceRegistrationTests
{
    [Fact]
    public void AddCoreResilience_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddCoreResilience();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddCoreResilience_ShouldRegisterPipelineProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreResilience();

        // Assert
        var descriptor = Assert.Single(
            services,
            x => x.ServiceType == typeof(IResiliencePipelineProvider));

        Assert.Equal(
            typeof(ResiliencePipelineProvider),
            descriptor.ImplementationType);

        Assert.Equal(
            ServiceLifetime.Singleton,
            descriptor.Lifetime);
    }

    [Fact]
    public void AddCoreResilience_ShouldRegisterPipelineRegistry()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreResilience();

        // Assert
        var descriptor = Assert.Single(
            services,
            x => x.ServiceType == typeof(PipelineRegistry));

        Assert.Equal(
            typeof(PipelineRegistry),
            descriptor.ImplementationType);

        Assert.Equal(
            ServiceLifetime.Singleton,
            descriptor.Lifetime);
    }

    [Fact]
    public void AddCoreResilience_ShouldRegisterPipelineBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreResilience();

        // Assert
        var descriptor = Assert.Single(
            services,
            x => x.ServiceType == typeof(IPipelineBuilder));

        Assert.Equal(
            typeof(PipelineBuilder),
            descriptor.ImplementationType);

        Assert.Equal(
            ServiceLifetime.Singleton,
            descriptor.Lifetime);
    }

    [Fact]
    public void AddCoreResilience_ShouldRegisterDefaultResilienceOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreResilience();

        using var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<IOptions<ResilienceOptions>>();

        Assert.NotNull(options);
    }

    [Fact]
    public void AddCoreResilience_ShouldConfigureOptions_WhenConfigureIsProvided()
    {
        // Arrange
        var services = new ServiceCollection();

        var expected = new ResilienceOptions();

        ConfigureDefaultRedisPipeline(expected);

        // Act
        services.AddCoreResilience(options =>
        {
            options.CopyFrom(expected);
        });

        using var provider = services.BuildServiceProvider();

        // Assert
        var configured = provider
            .GetRequiredService<IOptions<ResilienceOptions>>()
            .Value;

        Assert.True(configured.ContainsPipeline(PipelineType.Redis));
    }

    private static void ConfigureDefaultRedisPipeline(
    ResilienceOptions resilience)
    {
        resilience.AddPipeline(PipelineType.Redis, pipeline =>
        {
            pipeline.Retry = new RetryOptions
            {
                MaxRetryAttempts = 1,
                Delay = TimeSpan.FromMilliseconds(100),
                BackoffType = BackoffType.Constant
            }
            .Handle<RedisConnectionException>()
            .Handle<RedisTimeoutException>()
            .Handle<TimeoutException>();

            pipeline.CircuitBreaker = new CircuitBreakerOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 2,
                BreakDuration = TimeSpan.FromSeconds(15)
            }
            .Handle<RedisConnectionException>()
            .Handle<RedisTimeoutException>()
            .Handle<TimeoutException>();

            pipeline.Timeout = new TimeoutOptions
            {
                Timeout = TimeSpan.FromSeconds(2)
            };
        });
    }
}