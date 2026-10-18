using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Core.Observability.UnitTests;

/// <summary>
/// Builds a <see cref="WebApplicationBuilder"/> backed by an in-memory configuration source so each
/// test controls the <c>OpenTelemetry</c> section without touching appsettings files or the file system.
/// </summary>
internal static class ObservabilityTestBuilder
{
    private const string DefaultOtlpEndpoint = "http://otel-collector:4317";

    public static WebApplicationBuilder Create(params KeyValuePair<string, string?>[] settings)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection(settings);
        return builder;
    }

    /// <summary>Builds a configuration tree that turns tracing on with a reachable OTLP endpoint.</summary>
    public static WebApplicationBuilder CreateWithTracingEnabled(
        string endpoint = DefaultOtlpEndpoint,
        string samplingProbability = "1.0",
        string captureSqlStatements = "false") =>
        Create(
            Pair("OpenTelemetry:Tracing:Enabled", "true"),
            Pair("OpenTelemetry:Tracing:OtlpEndpoint", endpoint),
            Pair("OpenTelemetry:Tracing:SamplingProbability", samplingProbability),
            Pair("OpenTelemetry:Tracing:CaptureSqlStatements", captureSqlStatements));

    /// <summary>Builds a configuration tree that turns metrics on with a reachable OTLP endpoint.</summary>
    public static WebApplicationBuilder CreateWithMetricsEnabled(
        string endpoint = DefaultOtlpEndpoint,
        params string[] meters) =>
        Create(
        [
            Pair("OpenTelemetry:Metrics:Enabled", "true"),
            Pair("OpenTelemetry:Metrics:OtlpEndpoint", endpoint),
            .. meters.Select((meter, index) =>
                Pair($"OpenTelemetry:Metrics:Meters:{index}", meter))
        ]);

    /// <summary>Builds a configuration tree that enables the Serilog OTLP log sink.</summary>
    public static WebApplicationBuilder CreateWithLoggingEnabled(string endpoint = DefaultOtlpEndpoint) =>
        Create(
            Pair("OpenTelemetry:Logging:Enabled", "true"),
            Pair("OpenTelemetry:Logging:OtlpEndpoint", endpoint));

    private static KeyValuePair<string, string?> Pair(string key, string? value) =>
        new(key, value);
}