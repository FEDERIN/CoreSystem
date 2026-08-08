using Core.Idempotency.Abstractions;
using Core.Idempotency.PostgreSql.IntegrationTests.Fixtures;
using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Core.Idempotency.PostgreSql.IntegrationTests.Storage;

public sealed class PostgreSqlIdempotencyStorageTests(
    PostgreSqlContainerFixture fixture)
    : IClassFixture<PostgreSqlContainerFixture>
{
    private readonly PostgreSqlIdempotencyTestBaseImpl _fixture = new(
        fixture,
        ConfigureEndpoints);

    [Fact]
    public async Task GetAsync_Should_Throw_When_Fingerprint_Has_No_HashAlgorithm()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();

        await using var connection = new NpgsqlConnection(
            fixture.ConnectionString);

        await connection.OpenAsync(TestContext.Current.CancellationToken);

        await connection.ExecuteAsync(
            """
            INSERT INTO idempotency_keys
            (
                key,
                request_fingerprint,
                hash_algorithm,
                status_code,
                content_type,
                headers,
                body,
                expires_at
            )
            VALUES
            (
                @Key,
                'fingerprint',
                NULL,
                200,
                'application/json',
                '{}',
                @Body,
                NOW() + INTERVAL '1 day'
            );
            """,
            new
            {
                Key = key,
                Body = Array.Empty<byte>()
            });

        var storage = _fixture.Services
            .GetRequiredService<IIdempotencyStorage>();

        // Act
        var action = () => storage.GetAsync(key);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Stored fingerprint does not contain a hash algorithm.");
    }

    private static void ConfigureEndpoints(
        IEndpointRouteBuilder endpoints)
    {
    }

    private sealed class PostgreSqlIdempotencyTestBaseImpl(
        PostgreSqlContainerFixture fixture,
        Action<IEndpointRouteBuilder> configureEndpoints)
        : PostgreSqlIdempotencyTestBase(
            fixture,
            configureEndpoints)
    {
        public new IServiceProvider Services => base.Services;
    }
}