using Core.Observability.Abstractions;
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
public static class OpenTelemetryTracingExtensions
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

        var oTelOptions = new OpenTelemetryOptions();
        var section = configuration.GetSection(OpenTelemetryOptions.SectionName);

        // If the configuration section is missing, bypass tracing setup to prevent crashes
        if (!section.Exists()) return services;

        section.Bind(oTelOptions);

        // Early return if tracing is not enabled in configuration
        if (oTelOptions.Tracing == null || !oTelOptions.Tracing.Enabled)
            return services;

        // Configure the Resource with application metadata and versioning
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: serviceName ?? "unknown-service",
                serviceVersion: typeof(OpenTelemetryTracingExtensions).Assembly.GetName().Version?.ToString() ?? "1.0.0")
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = environment ?? "unknown",
                ["host.name"] = Environment.MachineName,
                ["service.namespace"] = serviceNamespace ?? "default-namespace"
            });

        var contributorTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(t => typeof(IObservabilityContributor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        var allSources = new List<string>();
        foreach (var type in contributorTypes)
        {
            var contributor = (IObservabilityContributor)Activator.CreateInstance(type)!;
            allSources.AddRange(contributor.GetActivitySources());
        }

        // Setup OpenTelemetry SDK for Tracing
        services.AddOpenTelemetry()
            .WithTracing(tracerProvider =>
            {
                tracerProvider
                    .SetResourceBuilder(resourceBuilder)
                    // Use the SamplingProbability from our centralized options (Safe default to 1.0)
                    .SetSampler(new TraceIdRatioBasedSampler(oTelOptions.Tracing.SamplingProbability))

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
                        options.EnrichWithSqlCommand = (activity, command) =>
                        {
                            if (command is SqlCommand sqlCommand)
                            {
                                var query = sqlCommand.CommandText;

                                // Redact sensitive information from traces
                                if (!string.IsNullOrEmpty(query) && query.Contains("password", StringComparison.OrdinalIgnoreCase))
                                {
                                    activity.SetTag("db.statement", "REDACTED_FOR_SECURITY");
                                }
                                else
                                {
                                    activity.SetTag("db.statement", query);
                                }
                            }
                        };
                    });

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
}