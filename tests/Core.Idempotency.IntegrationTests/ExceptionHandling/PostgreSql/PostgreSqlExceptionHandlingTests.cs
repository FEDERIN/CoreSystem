using Core.Idempotency.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Core.Idempotency.IntegrationTests.ExceptionHandling.PostgreSql;

public sealed class PostgreSqlExceptionHandlingTests(PostgreSqlContainerFixture fixture)
    : ExceptionHandlingTestsBase,
      IClassFixture<PostgreSqlContainerFixture>
{
    private readonly PostgreSqlExceptionHandlingTestBaseImpl _fixture = new(
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

    private sealed class PostgreSqlExceptionHandlingTestBaseImpl(
        PostgreSqlContainerFixture fixture,
        Action<IEndpointRouteBuilder> configureEndpoints)
        : PostgreSqlIdempotencyTestBase(fixture, configureEndpoints)
    {
        public new HttpClient Client => base.Client;
    }

    private sealed record CreateOrderRequest(string Name);
}