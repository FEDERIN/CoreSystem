using Core.Idempotency.PostgreSql.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Core.Idempotency.PostgreSql.IntegrationTests.Storage;

public sealed class PostgreSqlIdempotencyTests(PostgreSqlContainerFixture fixture)
    : IdempotencyTestsBase,
      IClassFixture<PostgreSqlContainerFixture>
{
    private readonly PostgreSqlIdempotencyTestBaseImpl _fixture = new(
        fixture,
        ConfigureEndpoints);

    protected override HttpClient Client => _fixture.Client;

    private static void ConfigureEndpoints(
        IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/orders", () =>
            Results.Ok(new
            {
                Id = Guid.NewGuid()
            }));
    }

    private sealed class PostgreSqlIdempotencyTestBaseImpl(
        PostgreSqlContainerFixture fixture,
        Action<IEndpointRouteBuilder> configureEndpoints)
        : PostgreSqlIdempotencyTestBase(
            fixture,
            configureEndpoints)
    {
        public new HttpClient Client => base.Client;
    }
}