using Core.Observability.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Diagnostics;

namespace Core.Observability.Extensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry metrics with OTLP export.
/// </summary>
internal static class OpenTelemetryMetricsExtensions
{
    public static IServiceCollection AddOpenTelemetryMetrics(
        this IServiceCollection services,
        IConfiguration configuration,
        string environment,
        string serviceName)
    {
        // Guard clause to ensure services collection is not null
        ArgumentNullException.ThrowIfNull(services);

        // Bind only the Metrics subsection: an invalid value in the Logging or Tracing sections must not
        // affect metrics. A missing section yields defaults, which are disabled, so metrics are bypassed.
        bool metricsBound = OpenTelemetryConfigurationBinder.TryBindChild<OtlpMetricsOptions>(
            configuration, nameof(OpenTelemetryOptions.Metrics), out OtlpMetricsOptions metricsOptions);

        if (!metricsBound)
        {
            // The section exists but did not bind. Keep metrics enabled when the flag itself is valid, so a
            // malformed sibling value (a bad meter name, for instance) does not silently disable the exporter.
            metricsOptions.Enabled = ReadEnabledFlag(configuration, nameof(OpenTelemetryOptions.Metrics));
        }

        var oTelOptions = new OpenTelemetryOptions { Metrics = metricsOptions };

        // Early exit if metrics are null or explicitly disabled in configuration
        if (oTelOptions.Metrics == null || !oTelOptions.Metrics.Enabled)
            return services;

        // Build the resource attributes that will be attached to all metrics
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName ?? "unknown-service")
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = environment ?? "unknown",
                ["host.name"] = Environment.MachineName
            });

        foreach (var contributor in ObservabilityContributorRegistry.GetRegistered(services))
        {
            contributor.ConfigureObservability(services, configuration);
        }

        services.AddOpenTelemetry()
            .WithMetrics(meterProvider =>
            {
                meterProvider
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                // Register custom meters from the configuration list (e.g., Idempotency meters)
                // We filter out null or whitespace entries to prevent runtime errors
                if (oTelOptions.Metrics.Meters != null)
                {
                    foreach (var meterName in oTelOptions.Metrics.Meters.Where(m => !string.IsNullOrWhiteSpace(m)))
                    {
                        meterProvider.AddMeter(meterName);
                    }
                }

                // Resilience check: Validate the OTLP endpoint before adding the exporter
                var endpoint = oTelOptions.Metrics.OtlpEndpoint;

                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    // Fail-safe: If no endpoint is provided, the app continues without metrics export
                    Trace.WriteLine("[Observability] Metrics OTLP Endpoint is empty. Skipping exporter.");
                    return;
                }

                // Using TryCreate to avoid UriFormatException and ensure the app remains stable
                if (Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
                {
                    meterProvider.AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = endpointUri;
                        opt.Protocol = OtlpExportProtocol.Grpc;
                    });
                }
                else
                {
                    // Fail-safe: Log a warning for invalid URL format but allow the host to start
                    Trace.WriteLine($"[Observability] Metrics OTLP Endpoint '{endpoint}' is invalid. Skipping exporter.");
                }
            });

        return services;
    }

    /// <summary>
    /// Reads the <c>Enabled</c> flag of a child section without binding the rest of it, so a malformed sibling
    /// value cannot hide the fact that the feature was explicitly turned on.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="childName">The child section name, for example <c>Metrics</c>.</param>
    /// <returns>The configured flag, or <c>false</c> when it is absent or not a valid boolean.</returns>
    private static bool ReadEnabledFlag(IConfiguration configuration, string childName) =>
        bool.TryParse(
            configuration[$"{OpenTelemetryOptions.SectionName}:{childName}:Enabled"], out bool enabled)
            && enabled;
}
