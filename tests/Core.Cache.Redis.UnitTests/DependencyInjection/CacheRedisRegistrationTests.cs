using Core.Cache.Abstractions;
using Core.Cache.Options;
using Core.Cache.Pipeline.Behaviors;
using Core.Cache.Redis.Builders;
using Core.Cache.Redis.DependencyInjection;
using Core.Cache.Redis.Diagnostics;
using Core.Cache.Redis.Storage.Abstractions;
using Core.Cache.Storage.Abstractions;
using Core.Observability.Abstractions;
using Core.Resilience.Abstractions;
using Core.Resilience.Options;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Cache.Redis.UnitTests.DependencyInjection;

public sealed class CacheRedisRegistrationTests
{
    [Fact]
    public void AddCoreCacheRedis_Should_Throw_WhenConfigureIsNull()
    {
        var services = new ServiceCollection();

        IServiceCollection action() =>
            services.AddCoreCacheRedis(null!);

        Assert.Throws<ArgumentNullException>((Func<IServiceCollection>)action);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_Throw_WhenCacheOptionsAreNotRegistered()
    {
        var services = new ServiceCollection();

        IServiceCollection action() =>
            services.AddCoreCacheRedis(options =>
            {
                options.Configuration = _ => {};
            });

        var exception = Assert.Throws<InvalidOperationException>(
            (Func<IServiceCollection>)action);

        Assert.Equal(
            "Cache options are required when using the Redis cache provider.",
            exception.Message);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_ReturnServices_WhenCacheIsDisabled()
    {
        var services = new ServiceCollection();

        services.AddSingleton(
            new CacheOptions
            {
                Enabled = false
            });

        var result = services.AddCoreCacheRedis(options =>
        {
            options.Configuration = _ =>
            {
            };
        });

        Assert.Same(
            services,
            result);

        Assert.DoesNotContain(
            services,
            x => x.ServiceType ==
                 typeof(IExternalCacheStorage));
    }

    [Fact]
    public void AddCoreCacheRedis_Should_Throw_WhenRedisConfigurationIsMissing()
    {
        var services = new ServiceCollection();

        services.AddSingleton(
            new CacheOptions
            {
                Enabled = true
            });

        IServiceCollection action() =>
            services.AddCoreCacheRedis(_ =>
            {
            });

        var exception = Assert.Throws<InvalidOperationException>(
            (Func<IServiceCollection>)action);

        Assert.Equal(
            "Redis configuration is required when using the Redis cache provider.",
            exception.Message);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_ReturnServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(
            new CacheOptions
            {
                Enabled = true
            });

        var result = services.AddCoreCacheRedis(options =>
        {
            options.Configuration = _ =>
            {
            };
        });

        Assert.Same(
            services,
            result);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_Register_ConnectionMultiplexer()
    {
        var services = CreateServices();

        var provider = services.BuildServiceProvider();

        var redis =
            provider.GetRequiredService<IConnectionMultiplexer>();

        Assert.NotNull(redis);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_Register_RedisHealthState()
    {
        var services = CreateServices();

        var provider = services.BuildServiceProvider();

        var state =
            provider.GetRequiredService<RedisHealthState>();

        Assert.NotNull(state);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_Register_HealthState()
    {
        var services = CreateServices();

        var provider = services.BuildServiceProvider();

        var state =
            provider.GetRequiredService<IHealthState>();

        Assert.NotNull(state);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_Register_PrimaryHealthStateWriter()
    {
        var services = CreateServices();

        var provider = services.BuildServiceProvider();

        var state =
            provider.GetRequiredService<IPrimaryHealthStateWriter>();

        Assert.NotNull(state);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_Register_HealthContributor()
    {
        var services = CreateServices();

        var provider = services.BuildServiceProvider();

        var contributors =
            provider.GetServices<IHealthCheckContributor>();

        Assert.Contains(
            contributors,
            x => x is RedisHealthContributor);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_Register_KeyBuilder()
    {
        var services = CreateServices();

        var provider = services.BuildServiceProvider();

        var keyBuilder =
            provider.GetRequiredService<IKeyBuilder>();

        Assert.NotNull(keyBuilder);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_Apply_InstanceName_To_KeyBuilder()
    {
        var services = new ServiceCollection();

        services.AddSingleton(
            new CacheOptions
            {
                Enabled = true,
                InstanceName = "my-app"
            });

        services.AddCoreCacheRedis(options =>
        {
            options.Configuration = configuration =>
            {
                configuration.EndPoints.Add(
                    "localhost:6379");
            };
        });

        var provider = services.BuildServiceProvider();

        var keyBuilder =
            provider.GetRequiredService<IKeyBuilder>();

        var key =
            keyBuilder.BuildCacheKey("users:1");

        Assert.Equal(
            "my-app:users:1",
            key);
    }

    [Fact]
    public void AddCoreCacheRedis_Should_RegisterResilienceBehavior_WhenRedisPipelineIsConfigured()
    {
        var services = new ServiceCollection();

        services.AddSingleton(
            new CacheOptions
            {
                Enabled = true
            });

        services.AddSingleton(
            new ResilienceOptions
            {
                Pipelines =
                {
                [PipelineType.Redis] = new PipelineOptions()
                }
            });

        services.AddCoreCacheRedis(options =>
        {
            options.Configuration = configuration =>
            {
                configuration.EndPoints.Add("localhost:6379");
            };
        });

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(ResilienceBehavior));
    }

    private static ServiceCollection CreateServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(
            new CacheOptions
            {
                Enabled = true
            });

        services.AddCoreCacheRedis(options =>
        {
            options.Configuration = configuration =>
            {
                configuration.EndPoints.Add(
                    "localhost:6379");
            };
        });

        return services;
    }
}