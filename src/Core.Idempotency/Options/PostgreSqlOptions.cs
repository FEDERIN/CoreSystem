namespace Core.Idempotency.Options;

public sealed class PostgreSqlOptions
{
    /// <summary>
    /// Connection string used by the PostgreSQL provider.
    /// </summary>
    public string? ConnectionString { get; set; }
}