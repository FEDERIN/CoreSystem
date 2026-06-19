using Core.Observability.Abstractions;
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
public static class OpenTelemetryMetricsExtensions
{
    public static IServiceCollection AddOpenTelemetryMetrics(
        this IServiceCollection services,
        IConfiguration configuration,
        string environment,
        string serviceName)
    {
        // Guard clause to ensure services collection is not null
        ArgumentNullException.ThrowIfNull(services);

        var oTelOptions = new OpenTelemetryOptions();
        var section = configuration.GetSection(OpenTelemetryOptions.SectionName);

        // If the configuration section is missing, we bypass OTel setup to prevent crashes
        if (!section.Exists())
        {
            return services;
        }

        section.Bind(oTelOptions);

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

        // 1. Escanear contribuciones (Igual que en Tracing)
        var contributorTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => typeof(IObservabilityContributor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        // 2. Ejecutar la configuración de métricas de cada módulo
        foreach (var type in contributorTypes)
        {
            var contributor = (IObservabilityContributor)Activator.CreateInstance(type)!;
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
}