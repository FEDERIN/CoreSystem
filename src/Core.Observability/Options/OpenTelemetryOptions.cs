namespace Core.Observability.Options;

/// <summary>Top-level options for OpenTelemetry logging, tracing and metrics export.</summary>
public class OpenTelemetryOptions
{
    /// <summary>The configuration section name used to bind these options.</summary>
    public const string SectionName = "OpenTelemetry";

    /// <summary>Logging export options.</summary>
    public OtlpLoggingOptions Logging { get; set; } = new();

    /// <summary>Tracing export options.</summary>
    public OtlpTracingOptions Tracing { get; set; } = new();

    /// <summary>Metrics export options.</summary>
    public OtlpMetricsOptions Metrics { get; set; } = new();
}

/// <summary>Options for exporting logs over OTLP.</summary>
public class OtlpLoggingOptions
{
    /// <summary>Enables OTLP log export.</summary>
    public bool Enabled { get; set; }

    /// <summary>OTLP endpoint used to export logs.</summary>
    public string OtlpEndpoint { get; set; } = "http://otel-collector:4317";
}

/// <summary>Options for exporting traces over OTLP.</summary>
public class OtlpTracingOptions
{
    /// <summary>Enables OTLP trace export.</summary>
    public bool Enabled { get; set; }

    /// <summary>OTLP endpoint used to export traces.</summary>
    public string OtlpEndpoint { get; set; } = "http://otel-collector:4317";

    /// <summary>Head-based sampling probability between 0 and 1.</summary>
    public double SamplingProbability { get; set; } = 1.0;

    /// <summary>Captures SQL statements in database spans when enabled.</summary>
    public bool CaptureSqlStatements { get; set; }
}

/// <summary>Options for exporting metrics over OTLP.</summary>
public class OtlpMetricsOptions
{
    /// <summary>Enables OTLP metrics export.</summary>
    public bool Enabled { get; set; }

    /// <summary>OTLP endpoint used to export metrics.</summary>
    public string OtlpEndpoint { get; set; } = "http://otel-collector:4317";

    /// <summary>Additional meter names to include in the export.</summary>
    public List<string> Meters { get; set; } = [];
}
