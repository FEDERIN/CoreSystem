using Core.Idempotency.Redis.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Core.Idempotency.Redis.IntegrationTests.Storage;

public sealed class RedisIdempotencyTests
    (RedisContainerFixture fixture)
        : IdempotencyTestsBase,
      IClassFixture<RedisContainerFixture>
{
    private readonly RedisIdempotencyTestBaseImpl _fixture = new(
            fixture,
            ConfigureEndpoints);

    protected override HttpClient Client => _fixture.Client;

    private static void ConfigureEndpoints(
        IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/orders", () =>
            Results.Ok(new { Id = Guid.NewGuid() }));
    }

    private sealed class RedisIdempotencyTestBaseImpl(
        RedisContainerFixture fixture,
        Action<IEndpointRouteBuilder> configureEndpoints)
                : RedisIdempotencyTestBase(fixture, configureEndpoints)
    {
        public new HttpClient Client => base.Client;
    }
}