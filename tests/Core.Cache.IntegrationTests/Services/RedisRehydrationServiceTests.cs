using Core.Cache.Abstractions;
using Core.Cache.DependencyInjection;
using Core.Cache.Services.Rehydration;
using Core.Cache.Storage.Memory;
using Core.Cache.Storage.Redis;
using Core.Cache.IntegrationTests.Cache;
using Core.Cache.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.IntegrationTests.Services;

public sealed class RedisRehydrationServiceTests(
    RedisContainerFixture fixture)
    : IClassFixture<RedisContainerFixture>
{
    private readonly IServiceProvider _provider = CreateProvider(fixture);

    [Fact]
    public async Task ExecuteCycleAsync_WhenRedisIsHealthy_ShouldDoNothing()
    {
        // Arrange
        var service = _provider.GetRequiredService<IRedisRehydrationService>();

        // Act
        var action = () => service.ExecuteCycleAsync(TestContext.Current.CancellationToken);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExecuteCycleAsync_ShouldIgnoreMemoryEntries()
    {
        // Arrange

        var service =
            _provider.GetRequiredService<IRedisRehydrationService>();

        var memory =
            _provider.GetRequiredService<MemoryStorage>();

        var redis =
            _provider.GetRequiredService<RedisStorage>();

        await memory.SetAsync(
            "memory-only",
            new CustomerDto
            {
                Id = 1,
                Name = "Memory"
            },
            null,
            TimeSpan.FromMilliseconds(300),
            ["customers"],
            TestContext.Current.CancellationToken);

        // Act

        await service.ExecuteCycleAsync(TestContext.Current.CancellationToken);

        // Assert

        var value =
            await redis.GetAsync<CustomerDto>("memory-only", TestContext.Current.CancellationToken);

        value.Should().BeNull();
    }

    private static ServiceProvider CreateProvider(
        RedisContainerFixture fixture)
    {
        var services = new ServiceCollection();

        services.AddCoreCache(options =>
        {
            options.DefaultProvider = CacheProviderType.Redis;

            options.Redis.Configuration = config =>
            {
                config.EndPoints.Add(fixture.ConnectionString);
            };
        });

        services.AddHealthChecks();

        return services.BuildServiceProvider();
    }
}