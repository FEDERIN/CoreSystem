using Core.Cache.Abstractions;
using Core.Cache.DependencyInjection;
using Core.Cache.Diagnostics;
using Core.Cache.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.Cache.IntegrationTests.Diagnostics;

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

        services.AddCoreCache(options =>
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