namespace Core.Observability.Correlation;

/// <summary>Configures correlation identifiers for incoming HTTP requests.</summary>
public sealed class CorrelationOptions
{
    public const string SectionName = "Core:Observability:Correlation";

    public bool Enabled { get; set; } = true;

    public string HeaderName { get; set; } = "X-Correlation-Id";

    public bool IncludeInResponse { get; set; } = true;

    /// <summary>Maximum accepted length for a client-provided identifier.</summary>
    public int MaximumLength { get; set; } = 128;
}
