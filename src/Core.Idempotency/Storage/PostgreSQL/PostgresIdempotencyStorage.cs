using Core.Idempotency.Abstractions;
using Core.Idempotency.Models;
using Core.Idempotency.Options;
using Dapper;
using Npgsql;

namespace Core.Idempotency.Storage.PostgreSQL;

internal sealed class PostgresIdempotencyStorage(
    IdempotencyOptions options)
    : IIdempotencyStorage
{
    private readonly string _connectionString =
        options.PostgreSql.ConnectionString!;

    public async Task<IdempotencyResponse?> GetAsync(
        string key,
        CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT
                status_code AS StatusCode,
                content_type AS ContentType,
                body AS Body
            FROM idempotency_keys
            WHERE key = @key
              AND expires_at > NOW();
            """;

        return await conn.QueryFirstOrDefaultAsync<IdempotencyResponse>(
            new CommandDefinition(
                sql,
                new { key },
                cancellationToken: ct));
    }

    public async Task SetAsync(
        string key,
        IdempotencyResponse response,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        const string sql = """
            INSERT INTO idempotency_keys
            (
                key,
                status_code,
                content_type,
                body,
                expires_at
            )
            VALUES
            (
                @Key,
                @StatusCode,
                @ContentType,
                @Body,
                NOW() + @Expiration
            )
            ON CONFLICT (key)
            DO UPDATE SET
                status_code = EXCLUDED.status_code,
                content_type = EXCLUDED.content_type,
                body = EXCLUDED.body,
                expires_at = EXCLUDED.expires_at;
            """;

        await conn.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    Key = key,
                    response.StatusCode,
                    response.ContentType,
                    response.Body,
                    Expiration = expiration ?? TimeSpan.FromDays(1)
                },
                cancellationToken: ct));
    }
}