using Core.Idempotency.Abstractions;
using Core.Idempotency.Diagnostics;
using Core.Idempotency.Models;
using Core.Idempotency.Options;
using Core.Idempotency.Storage.PostgreSQL.Models;
using Core.Serialization.Abstractions;
using Dapper;
using Npgsql;
using System.Diagnostics;

namespace Core.Idempotency.Storage.PostgreSQL;

internal sealed class PostgresIdempotencyStorage(
    IdempotencyOptions options,
    IPayloadSerializer serializer,
    IdempotencyMetrics metrics)
    : IIdempotencyStorage
{
    private readonly string _connectionString =
        options.PostgreSql.ConnectionString!;

    public async Task<IdempotencyEntry?> GetAsync(
        string key,
        CancellationToken ct = default)
    {
        const string sql = """
        SELECT
            request_fingerprint AS RequestFingerprint,
            hash_algorithm AS HashAlgorithm,
            status_code AS StatusCode,
            content_type AS ContentType,
            headers AS Headers,
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

            var record = await conn.QueryFirstOrDefaultAsync<PostgreSqlIdempotencyRecord>(
                new CommandDefinition(
                    sql,
                    new { Key = key },
                    cancellationToken: ct));

            if (record is null)
            {
                return null;
            }

            if (record.RequestFingerprint is not null &&
                string.IsNullOrWhiteSpace(record.HashAlgorithm))
            {
                throw new InvalidOperationException(
                    "Stored fingerprint does not contain a hash algorithm.");
            }

            RequestFingerprint? fingerprint = null;

            if (record.RequestFingerprint is not null)
            {
                fingerprint = new RequestFingerprint
                {
                    HashAlgorithm = record.HashAlgorithm!,
                    Value = record.RequestFingerprint
                };
            }

            return new IdempotencyEntry
            {
                RequestFingerprint = fingerprint,
                Response = new IdempotencyResponse
                {
                    StatusCode = record.StatusCode,
                    ContentType = record.ContentType,
                    Body = record.Body,
                    Headers = serializer.Deserialize<Dictionary<string, string[]>>(record.Headers)
                              ?? []
                }
            };
        }
        finally
        {
            metrics.RecordStorageReadDuration(
                Stopwatch.GetElapsedTime(start).TotalMilliseconds);
        }
    }

    public async Task SetAsync(
        string key,
        IdempotencyEntry entry,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
    {
        const string sql = """
        INSERT INTO idempotency_keys
        (
            key,
            request_Fingerprint,
            hash_Algorithm,
            status_code,
            content_type,
            headers,
            body,
            expires_at
        )
        VALUES
        (
            @Key,
            @RequestFingerprint,
            @HashAlgorithm,
            @StatusCode,
            @ContentType,
            @Headers,
            @Body,
            NOW() + @Expiration
        )
        ON CONFLICT (key)
        DO UPDATE
        SET
            request_fingerprint = EXCLUDED.request_fingerprint,
            hash_algorithm = EXCLUDED.hash_algorithm,
            status_code = EXCLUDED.status_code,
            content_type = EXCLUDED.content_type,
            headers = EXCLUDED.headers,
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

            var response = entry.Response;

            var rows = await conn.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new
                    {
                        Key = key,
                        RequestFingerprint = entry.RequestFingerprint?.Value,
                        entry.RequestFingerprint?.HashAlgorithm,
                        response.StatusCode,
                        response.ContentType,
                        Headers = serializer.Serialize(response.Headers),
                        response.Body,
                        Expiration = expiresIn
                    },
                    cancellationToken: ct));

            if (rows > 0)
            {
                metrics.RecordStorageWrite();

                if (response.Body.Length > 0)
                {
                    metrics.RecordPayloadSize(
                        response.Body.Length);
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