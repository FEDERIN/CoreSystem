using Core.Observability.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Core.Observability.Extensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry metrics using Prometheus.
/// Metrics are exposed via a scraping endpoint.
/// </summary>
public static class PrometheusMetricsExtensions
{
    public static IServiceCollection AddPrometheusMetrics(
        this IServiceCollection services,
        IConfiguration configuration,
        string environment,
        string serviceName)
    {
        // Bind configuration section to OpenTelemetryOptions
        var oTelOptions = new OpenTelemetryOptions();
        configuration.GetSection(OpenTelemetryOptions.SectionName).Bind(oTelOptions);

        if (!oTelOptions.Metrics.Enabled)
            return services;

        // Configure the resource identity for the metrics
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: serviceName,
                serviceVersion: typeof(PrometheusMetricsExtensions).Assembly.GetName().Version?.ToString() ?? "1.0.0")
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = environment,
                ["host.name"] = Environment.MachineName
            });

        // Set up OpenTelemetry Metrics
        services.AddOpenTelemetry()
            .WithMetrics(meterProvider =>
            {
                meterProvider
                    .SetResourceBuilder(resourceBuilder)
                    // Instruments incoming ASP.NET Core requests
                    .AddAspNetCoreInstrumentation()
                    // Instruments outgoing HTTP calls
                    .AddHttpClientInstrumentation()
                    // Instruments .NET Runtime (GC, ThreadPool, etc.)
                    .AddRuntimeInstrumentation()
                    // Adds the Prometheus exporter (Default path is /metrics)
                    .AddPrometheusExporter();
            });

        return services;
    }

    /// <summary>
    /// Maps the Prometheus scraping endpoint to the web application.
    /// </summary>
    public static WebApplication MapPrometheusMetrics(this WebApplication app)
    {
        var oTelOptions = new OpenTelemetryOptions();
        app.Configuration.GetSection(OpenTelemetryOptions.SectionName).Bind(oTelOptions);

        if (oTelOptions.Metrics.Enabled)
        {
            // Enables the /metrics endpoint for Prometheus to scrape data
            app.UseOpenTelemetryPrometheusScrapingEndpoint();
        }

        return app;
    }
}