using Core.Idempotency.Models;
using Npgsql;
using Dapper;

namespace Core.Idempotency.Storage.PostgreSQL;

public class PostgresIdempotencyStorage(string connectionString) : IIdempotencyStorage
{
    private readonly string _connectionString = connectionString;
    private static bool _tableChecked = false;

    private async Task EnsureTableExistsAsync()
    {
        if (_tableChecked) return;

        using var conn = new NpgsqlConnection(_connectionString);
        const string sql = @"
            CREATE TABLE IF NOT EXISTS idempotency_keys (
                key TEXT PRIMARY KEY,
                status_code INT NOT NULL,
                content_type TEXT,
                body TEXT,
                created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                expires_at TIMESTAMPTZ NOT NULL
            );
            CREATE INDEX IF NOT EXISTS idx_idempotency_expires ON idempotency_keys(expires_at);";

        await conn.ExecuteAsync(sql);
        _tableChecked = true;
    }

    public async Task<IdempotencyResponse?> GetAsync(string key)
    {
        await EnsureTableExistsAsync();

        using var conn = new NpgsqlConnection(_connectionString);
        const string sql = @"
            SELECT status_code as StatusCode, 
                   content_type as ContentType, 
                   body as Body 
            FROM idempotency_keys 
            WHERE key = @key AND expires_at > NOW()";

        return await conn.QueryFirstOrDefaultAsync<IdempotencyResponse>(sql, new { key });
    }

    public async Task SaveAsync(string key, IdempotencyResponse response, TimeSpan expiration)
    {
        await EnsureTableExistsAsync();

        using var conn = new NpgsqlConnection(_connectionString);
        const string sql = @"
            INSERT INTO idempotency_keys (key, status_code, content_type, body, expires_at)
            VALUES (@key, @StatusCode, @ContentType, @Body, NOW() + @Expiration)
            ON CONFLICT (key) DO UPDATE SET 
                status_code = EXCLUDED.status_code, 
                body = EXCLUDED.body, 
                expires_at = EXCLUDED.expires_at;";

        await conn.ExecuteAsync(sql, new
        {
            key,
            response.StatusCode,
            response.ContentType,
            response.Body,
            Expiration = expiration
        });
    }
}