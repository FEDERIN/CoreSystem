namespace Core.Correlation;

/// <summary>Configures correlation identifiers for incoming HTTP requests.</summary>
public sealed class CorrelationOptions
{
    /// <summary>The configuration section name used to bind these options.</summary>
    public const string SectionName = "Core:Correlation";

    /// <summary>Enables correlation identifier propagation.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Header name carrying the correlation identifier.</summary>
    public string HeaderName { get; set; } = "X-Correlation-Id";

    /// <summary>Whether the correlation identifier is echoed back in the response.</summary>
    public bool IncludeInResponse { get; set; } = true;

    /// <summary>Maximum accepted length for a client-provided identifier.</summary>
    public int MaximumLength { get; set; } = 128;
}
