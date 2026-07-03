using Core.DistributedCache.Abstractions;
using Core.DistributedCache.DependencyInjection;
using Core.DistributedCache.Diagnostics;
using Core.DistributedCache.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.DistributedCache.IntegrationTests.Diagnostics;

public sealed class RedisHealthCheckTests(RedisContainerFixture fixture)
    : IClassFixture<RedisContainerFixture>
{
    private readonly IServiceProvider _provider = CreateProvider(fixture);

    [Fact]
    public async Task CheckHealthAsync_WhenRedisIsAvailable_ShouldReturnHealthy()
    {
        // Arrange
        var healthCheck = _provider.GetRequiredService<RedisHealthCheck>();

        // Act
        var result = await healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            TestContext.Current.CancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("Redis is connected successfully.");
    }

    [Fact]
    public async Task CheckHealthAsync_WhenHealthStateIsUnhealthy_ShouldReturnDegraded()
    {
        // Arrange
        var state = _provider.GetRequiredService<IRedisHealthState>();

        state.MarkUnhealthy();

        var healthCheck = _provider.GetRequiredService<RedisHealthCheck>();

        // Act
        var result = await healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            TestContext.Current.CancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Degraded);
        result.Description.Should().Contain("fallback");
    }

    [Fact]
    public async Task CheckHealthAsync_WhenHealthStateRecovers_ShouldReturnHealthy()
    {
        // Arrange
        var state = _provider.GetRequiredService<IRedisHealthState>();

        state.MarkUnhealthy();
        state.MarkHealthy();

        var healthCheck = _provider.GetRequiredService<RedisHealthCheck>();

        // Act
        var result = await healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            TestContext.Current.CancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }


    private static ServiceProvider CreateProvider(RedisContainerFixture fixture)
    {
        var services = new ServiceCollection();

        services.AddCoreDistributedCache(options =>
        {
            options.DefaultProvider = CacheProviderType.Redis;

            options.Redis.Configuration = cfg =>
            {
                cfg.EndPoints.Add(fixture.ConnectionString);
            };
        });

        services.AddHealthChecks();

        return services.BuildServiceProvider();
    }
}