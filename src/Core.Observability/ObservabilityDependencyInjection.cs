using Core.Observability.Extensions;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Core.Observability;


/// <summary>
/// Main entry point for observability services registration and middleware configuration.
/// Consolidates Logging (Serilog), Tracing (OpenTelemetry), and Metrics (Prometheus).
/// </summary>
public static class ObservabilityDependencyInjection
{
    /// <summary>
    /// Registers all observability services. 
    /// This method configures Serilog for the Host and adds OpenTelemetry services to the container.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder of the API.</param>
    /// <param name="environment">The current hosting environment name.</param>
    /// <returns>The updated WebApplicationBuilder.</returns>
    public static WebApplicationBuilder AddObservability(
        this WebApplicationBuilder builder,
        string environment,
        string serviceName,
        string serviceNamespace)
    {
        // 1. Configure Serilog for structured logging and OTLP export
        builder.AddSerilogConfiguration(environment, serviceName);

        // 2. Configure OpenTelemetry Distributed Tracing (OTLP/gRPC)
        builder.Services.AddOpenTelemetryTracing(builder.Configuration, environment, serviceName, serviceNamespace);

        // 3. Configure Prometheus Metrics collection
        builder.Services.AddOpenTelemetryMetrics(builder.Configuration, environment, serviceName);

        // 4. Add Health Checks (optional, but recommended for readiness/liveness probes)
        builder.Services.AddCoreHealthChecks();

        return builder;
    }

    /// <summary>
    /// Configures observability middlewares and maps the metrics scraping endpoint.
    /// Should be called after app.Build().
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <returns>The updated WebApplication.</returns>
    public static WebApplication UseObservabilityEndpoints(this WebApplication app)
    {
        // Enable Serilog request logging to capture HTTP metadata (status codes, duration, etc.)
        app.UseSerilogRequestLogging();


        // Maps health check endpoints (/health and /ready)
        app.MapCoreHealthEndpoints();

        return app;
    }
}