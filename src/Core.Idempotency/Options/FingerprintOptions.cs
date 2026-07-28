namespace Core.Idempotency.Options;

public sealed class FingerprintOptions
{
    /// <summary>
    /// Enables request fingerprint validation.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Includes the query string in the fingerprint.
    /// </summary>
    public bool IncludeQueryString { get; set; }

    /// <summary>
    /// Includes the Content-Type header in the fingerprint.
    /// </summary>
    public bool IncludeContentType { get; set; } = true;

    /// <summary>
    /// Additional request headers to include in the fingerprint.
    /// Header names are treated case-insensitively.
    /// </summary>
    public ISet<string> IncludedHeaders { get; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}