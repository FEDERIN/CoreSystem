using Core.Redis.Connection;
using Core.Redis.DependencyInjection;
using Core.Redis.Options;
using Core.Redis.Synchronization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Redis.UnitTests.DependencyInjection;

public class RedisRegistrationTests
{
    [Fact]
    public void AddCoreRedis_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddCoreRedis();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddCoreRedis_ShouldRegisterRedisConnectionFactory()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreRedis();

        // Assert
        var descriptor = Assert.Single(services, x =>
                x.ServiceType == typeof(IRedisConnectionFactory));

        Assert.Equal(
            typeof(RedisConnectionFactory),
            descriptor.ImplementationType);

        Assert.Equal(
            ServiceLifetime.Singleton,
            descriptor.Lifetime);
    }

    [Fact]
    public void AddCoreRedis_ShouldRegisterDistributedLockProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreRedis();

        // Assert
        var descriptor = Assert.Single(services, x =>
                x.ServiceType == typeof(IDistributedLockProvider));

        Assert.Equal(
            typeof(RedisLockProvider),
            descriptor.ImplementationType);

        Assert.Equal(
            ServiceLifetime.Singleton,
            descriptor.Lifetime);
    }

    [Fact]
    public void AddCoreRedis_ShouldRegisterDefaultRedisLockOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreRedis();

        using var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<RedisLockOptions>();

        Assert.NotNull(options);
    }

    [Fact]
    public void AddCoreRedis_ShouldConfigureRedisLockOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreRedis(options =>
        {
            options.LockDuration = TimeSpan.FromSeconds(10);
            options.RetryDelay = TimeSpan.FromMilliseconds(50);
            options.MaxWaitTime = TimeSpan.FromSeconds(5);
        });

        using var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<RedisLockOptions>();

        Assert.Equal(TimeSpan.FromSeconds(10), options.LockDuration);
        Assert.Equal(TimeSpan.FromMilliseconds(50), options.RetryDelay);
        Assert.Equal(TimeSpan.FromSeconds(5), options.MaxWaitTime);
    }
}