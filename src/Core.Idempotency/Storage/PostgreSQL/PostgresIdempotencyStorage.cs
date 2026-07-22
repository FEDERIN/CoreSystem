using Core.Idempotency.Abstractions;
using Core.Idempotency.Diagnostics;
using Core.Idempotency.Models;
using Core.Idempotency.Options;
using Dapper;
using Npgsql;
using System.Diagnostics;
using System.Text;

namespace Core.Idempotency.Storage.PostgreSQL;

internal sealed class PostgresIdempotencyStorage(
    IdempotencyOptions options,
    IdempotencyMetrics metrics)
    : IIdempotencyStorage
{
    private readonly string _connectionString =
        options.PostgreSql.ConnectionString!;

    public async Task<IdempotencyResponse?> GetAsync(
        string key,
        CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                status_code AS StatusCode,
                content_type AS ContentType,
                body AS Body
            FROM idempotency_keys
            WHERE key = @Key
              AND expires_at > NOW();
            """;

        long start = Stopwatch.GetTimestamp();

        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(ct);

            return await conn.QueryFirstOrDefaultAsync<IdempotencyResponse>(
                new CommandDefinition(
                    sql,
                    new { Key = key },
                    cancellationToken: ct));
        }
        finally
        {
            metrics.RecordStorageReadDuration(
                Stopwatch.GetElapsedTime(start).TotalMilliseconds);
        }
    }

    public async Task SetAsync(
        string key,
        IdempotencyResponse response,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
    {
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
            DO UPDATE
            SET
                status_code = EXCLUDED.status_code,
                content_type = EXCLUDED.content_type,
                body = EXCLUDED.body,
                expires_at = EXCLUDED.expires_at
            WHERE idempotency_keys.expires_at < NOW();
            """;

        var expiresIn = expiration ?? TimeSpan.FromDays(1);

        long start = Stopwatch.GetTimestamp();

        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(ct);

            var rows = await conn.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new
                    {
                        Key = key,
                        response.StatusCode,
                        response.ContentType,
                        response.Body,
                        Expiration = expiresIn
                    },
                    cancellationToken: ct));

            if (rows > 0)
            {
                metrics.RecordStorageWrite();

                if (!string.IsNullOrEmpty(response.Body))
                {
                    metrics.RecordPayloadSize(
                        Encoding.UTF8.GetByteCount(response.Body));
                }
            }
        }
        finally
        {
            metrics.RecordStorageWriteDuration(
                Stopwatch.GetElapsedTime(start).TotalMilliseconds);
        }
    }
}