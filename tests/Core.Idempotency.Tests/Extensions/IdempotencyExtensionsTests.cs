using Core.Idempotency.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core.Idempotency.Tests.Extensions;

public class IdempotencyExtensionsTests
{
    [Fact]
    public void AddRedisIdempotency_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var redisConnectionString = "localhost:6379";

        // Act
        services.AddRedisIdempotency(
            redis =>
            {
                redis.EndPoints.Add(redisConnectionString);
            },
            opts =>
            {
                opts.HeaderName = "X-Custom-Key";
                opts.Expiration = TimeSpan.FromHours(2);
            });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var storage = serviceProvider.GetService<IIdempotencyStorage>();
        storage.Should().NotBeNull();
        storage.Should().BeOfType<Idempotency.Storage.Redis.RedisIdempotencyStorage>();
    }

    [Fact]
    public void AddPostgresIdempotency_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var pgConnectionString = "Host=localhost;Database=test";

        // Act
        services.AddPostgresIdempotency(pgConnectionString);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var storage = serviceProvider.GetService<IIdempotencyStorage>();
        storage.Should().NotBeNull();
        storage.Should().BeOfType<Idempotency.Storage.PostgreSQL.PostgresIdempotencyStorage>();
    }

    [Fact]
    public void AddRedisIdempotency_WithCustomOptions_AppliesOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var redisConnectionString = "localhost:6379";

        // Act
        services.AddRedisIdempotency(
            redis =>
            {
                redis.EndPoints.Add(redisConnectionString);
            },
            opts =>
            {
                opts.HeaderName = "X-Custom-Key";
                opts.Expiration = TimeSpan.FromHours(2);
            });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsMonitor = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<Core.Idempotency.Options.IdempotencyOptions>>();
        var options = optionsMonitor.CurrentValue;
        options.HeaderName.Should().Be("X-Custom-Key");
        options.Expiration.Should().Be(TimeSpan.FromHours(2));
    }

    [Fact]
    public void AddPostgresIdempotency_WithCustomOptions_AppliesOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var pgConnectionString = "Host=localhost;Database=test";

        // Act
        services.AddPostgresIdempotency(
            pgConnectionString,
            opts =>
            {
                opts.AllowedMethods = ["POST", "PUT", "DELETE"];
                opts.MeterName = "custom.idempotency";
            });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var optionsMonitor = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<Idempotency.Options.IdempotencyOptions>>();
        var options = optionsMonitor.CurrentValue;
        options.AllowedMethods.Should().Contain("DELETE");
        options.MeterName.Should().Be("custom.idempotency");
    }
}