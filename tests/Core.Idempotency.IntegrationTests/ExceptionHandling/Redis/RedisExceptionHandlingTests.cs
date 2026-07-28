using Core.Idempotency.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Core.Idempotency.IntegrationTests.ExceptionHandling.Redis;

public sealed class RedisExceptionHandlingTests(RedisContainerFixture fixture)
    : ExceptionHandlingTestsBase,
      IClassFixture<RedisContainerFixture>
{
    private readonly RedisExceptionHandlingTestBaseImpl _fixture = new(
        fixture,
        ConfigureEndpoints);

    protected override HttpClient Client => _fixture.Client;

    private static void ConfigureEndpoints(
        IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/orders",
            (CreateOrderRequest request) =>
                Results.Ok(request));
    }

    private sealed class RedisExceptionHandlingTestBaseImpl(
        RedisContainerFixture fixture,
        Action<IEndpointRouteBuilder> configureEndpoints)
        : RedisIdempotencyTestBase(fixture, configureEndpoints)
    {
        public new HttpClient Client => base.Client;
    }

    private sealed record CreateOrderRequest(string Name);
}