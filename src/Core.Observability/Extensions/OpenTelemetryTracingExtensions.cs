using Core.Observability.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Core.Observability.Extensions;

/// <summary>
/// Extension methods for setting up OpenTelemetry tracing using a centralized options class.
/// </summary>
internal static class OpenTelemetryTracingExtensions
{
    public static IServiceCollection AddOpenTelemetryTracing(
        this IServiceCollection services,
        IConfiguration configuration,
        string environment,
        string serviceName,
        string serviceNamespace)
    {
        // Guard clause for the service collection
        ArgumentNullException.ThrowIfNull(services);

        // Bind only the Tracing subsection: an invalid value in the Logging or Metrics sections must not
        // affect tracing. A missing section yields defaults, which are disabled, so tracing is bypassed.
        bool tracingBound = OpenTelemetryConfigurationBinder.TryBindChild<OtlpTracingOptions>(
            configuration, nameof(OpenTelemetryOptions.Tracing), out OtlpTracingOptions tracingOptions);

        if (!tracingBound)
        {
            // The section exists but did not bind, so one value is malformed. Keep tracing enabled when the
            // flag itself is valid: dropping it to false would silently disable tracing because of a typo in
            // an unrelated value such as SamplingProbability. The remaining values fall back to defaults,
            // which the sampling guard below already handles.
            tracingOptions.Enabled = ReadEnabledFlag(configuration, nameof(OpenTelemetryOptions.Tracing));
        }

        var oTelOptions = new OpenTelemetryOptions { Tracing = tracingOptions };

        // Early return if tracing is not enabled in configuration
        if (oTelOptions.Tracing == null || !oTelOptions.Tracing.Enabled)
            return services;

        var samplingProbability = oTelOptions.Tracing.SamplingProbability;
        if (double.IsNaN(samplingProbability) ||
            samplingProbability < 0 ||
            samplingProbability > 1)
        {
            Trace.WriteLine(
                "[Observability] Tracing SamplingProbability must be between 0 and 1. Using 1.0.");
            samplingProbability = 1.0;
        }

        // Configure the Resource with application metadata and versioning
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: serviceName ?? "unknown-service",
                serviceVersion: System.Reflection.Assembly.GetEntryAssembly()
                    ?.GetName()
                    .Version
                    ?.ToString()
                    ?? typeof(OpenTelemetryTracingExtensions).Assembly.GetName().Version?.ToString()
                    ?? "1.0.0")
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = environment ?? "unknown",
                ["host.name"] = Environment.MachineName,
                ["service.namespace"] = serviceNamespace ?? "default-namespace"
            });

        var activitySources = ObservabilityContributorRegistry
            .GetRegistered(services)
            .SelectMany(contributor => contributor.GetActivitySources())
            .Where(source => !string.IsNullOrWhiteSpace(source))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        // Setup OpenTelemetry SDK for Tracing
        services.AddOpenTelemetry()
            .WithTracing(tracerProvider =>
            {
                tracerProvider
                    .SetResourceBuilder(resourceBuilder)
                    // Use the SamplingProbability from our centralized options (Safe default to 1.0)
                    .SetSampler(new TraceIdRatioBasedSampler(samplingProbability))

                    // Automatic instrumentation for ASP.NET Core requests
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        // Filter out noise from infrastructure endpoints
                        options.Filter = httpContext =>
                        {
                            var path = httpContext.Request.Path.Value ?? string.Empty;
                            return !path.Contains("/health") &&
                                   !path.Contains("/swagger") &&
                                   !path.Contains("/metrics");
                        };
                    })
                    // Automatic instrumentation for outgoing HTTP calls
                    .AddHttpClientInstrumentation()

                    // Automatic instrumentation for SQL Server queries with security redaction
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.RecordException = true;

                        if (!oTelOptions.Tracing.CaptureSqlStatements)
                        {
                            return;
                        }

                        options.EnrichWithSqlCommand = (activity, command) =>
                        {
                            if (command is SqlCommand sqlCommand)
                            {
                                activity.SetTag("db.statement", sqlCommand.CommandText);
                            }
                        };
                    });

                if (activitySources.Length > 0)
                {
                    tracerProvider.AddSource(activitySources);
                }

                // Resilience check for Tracing OTLP endpoint
                var endpoint = oTelOptions.Tracing.OtlpEndpoint;

                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    Trace.WriteLine("[Observability] Tracing OTLP Endpoint is empty. Skipping exporter.");
                    return;
                }

                // Using TryCreate to ensure the application remains stable despite configuration errors
                if (Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
                {
                    tracerProvider.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = endpointUri;
                        otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                    });
                }
                else
                {
                    Trace.WriteLine($"[Observability] Tracing OTLP Endpoint '{endpoint}' is invalid. Skipping exporter.");
                }
            });


        if (environment == "Development")
        {
            // Allow unencrypted gRPC traffic (required for some OTLP/gRPC local environments)
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        return services;
    }

    /// <summary>
    /// Reads the <c>Enabled</c> flag of a child section without binding the rest of it, so a malformed sibling
    /// value cannot hide the fact that the feature was explicitly turned on.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="childName">The child section name, for example <c>Tracing</c>.</param>
    /// <returns>The configured flag, or <c>false</c> when it is absent or not a valid boolean.</returns>
    private static bool ReadEnabledFlag(IConfiguration configuration, string childName) =>
        bool.TryParse(
            configuration[$"{OpenTelemetryOptions.SectionName}:{childName}:Enabled"], out bool enabled)
            && enabled;
}
