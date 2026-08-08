using System.Text;
using Core.Idempotency.Abstractions;
using Core.Idempotency.Models;
using Core.Idempotency.Redis.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.Redis.IntegrationTests.Storage;

public sealed class RedisIdempotencyStorageTests(
    RedisContainerFixture fixture)
    : IClassFixture<RedisContainerFixture>
{
    private readonly RedisIdempotencyTestBaseImpl _fixture = new(
        fixture,
        ConfigureEndpoints);

    [Fact]
    public async Task SetAsync_Should_Not_Overwrite_When_Key_Already_Exists()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();

        var storage = _fixture.Services
            .GetRequiredService<IIdempotencyStorage>();

        var entry = new IdempotencyEntry
        {
            Response = new IdempotencyResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = Encoding.UTF8.GetBytes("""{"value":1}"""),
                Headers = new Dictionary<string, string[]>()
            }
        };

        // Primera escritura
        await storage.SetAsync(key, entry, ct: TestContext.Current.CancellationToken);

        var newEntry = new IdempotencyEntry
        {
            Response = new IdempotencyResponse
            {
                StatusCode = 500,
                ContentType = "application/json",
                Body = Encoding.UTF8.GetBytes("""{"value":2}"""),
                Headers = new Dictionary<string, string[]>()
            }
        };

        // Act
        await storage.SetAsync(key, newEntry, ct: TestContext.Current.CancellationToken);

        // Assert
        var stored = await storage.GetAsync(key, TestContext.Current.CancellationToken);

        stored.Should().NotBeNull();
        stored!.Response.StatusCode.Should().Be(200);
        stored.Response.ContentType.Should().Be("application/json");
        stored.Response.Body.Should()
            .Equal(Encoding.UTF8.GetBytes("""{"value":1}"""));
    }

    private static void ConfigureEndpoints(
        IEndpointRouteBuilder endpoints)
    {
    }

    private sealed class RedisIdempotencyTestBaseImpl(
        RedisContainerFixture fixture,
        Action<IEndpointRouteBuilder> configureEndpoints)
        : RedisIdempotencyTestBase(
            fixture,
            configureEndpoints)
    {
        public new IServiceProvider Services => base.Services;
    }
}