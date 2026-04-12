using Core.Observability.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Core.Observability.Extensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry metrics using Prometheus.
/// Metrics are exposed via a scraping endpoint.
/// </summary>
public static class OpenTelemetryMetricsExtensions
{
    public static IServiceCollection AddOpenTelemetryMetrics(
    this IServiceCollection services,
    IConfiguration configuration,
    string environment,
    string serviceName)
    {
        var oTelOptions = new OpenTelemetryOptions();
        configuration.GetSection(OpenTelemetryOptions.SectionName).Bind(oTelOptions);

        if (!oTelOptions.Metrics.Enabled)
            return services;

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName)
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = environment,
                ["host.name"] = Environment.MachineName
            });

        services.AddOpenTelemetry()
            .WithMetrics(meterProvider =>
            {
                meterProvider
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    // In this configuration, we export metrics to an OpenTelemetry Collector instead of using the Prometheus exporter directly.
                    .AddOtlpExporter(options =>
                    {
                        // Configure the OTLP exporter to send metrics to the specified endpoint using gRPC.
                        options.Endpoint = new Uri(oTelOptions.Tracing.OtlpEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

        return services;
    }
}