namespace Core.Idempotency.Options;

/// <summary>
/// Configuration options for the PostgreSQL provider.
/// </summary>
public sealed class PostgreSqlOptions
{
    /// <summary>
    /// Connection string used by the PostgreSQL provider.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Copies the properties from the specified <see cref="PostgreSqlOptions"/> instance to this instance.
    /// </summary>
    /// <param name="source"></param>
    public void CopyFrom(PostgreSqlOptions source)
    {
        ArgumentNullException.ThrowIfNull(source);
        ConnectionString = source.ConnectionString;
    }
}