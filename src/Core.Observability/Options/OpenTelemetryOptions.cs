namespace Core.Observability.Options;

public class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public OtlpLoggingOptions Logging { get; set; } = new();
    public OtlpTracingOptions Tracing { get; set; } = new();
    public OtlpMetricsOptions Metrics { get; set; } = new();
}

public class OtlpLoggingOptions
{
    public bool Enabled { get; set; }
    public string OtlpEndpoint { get; set; } = "http://jaeger:4317";
}

public class OtlpTracingOptions
{
    public bool Enabled { get; set; }
    public string OtlpEndpoint { get; set; } = "http://jaeger:4317";
    public double SamplingProbability { get; set; } = 1.0;
}

public class OtlpMetricsOptions
{
    public bool Enabled { get; set; }
}