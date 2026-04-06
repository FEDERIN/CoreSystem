using Core.Observability.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
        // Bind the "OpenTelemetry" section from appsettings.json to our options class
        var oTelOptions = new OpenTelemetryOptions();
        configuration.GetSection(OpenTelemetryOptions.SectionName).Bind(oTelOptions);

        // Early return if tracing is not enabled in configuration
        if (!oTelOptions.Tracing.Enabled)
            return services;

        // Configure the Resource with application metadata
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: serviceName,
                serviceVersion: typeof(OpenTelemetryTracingExtensions).Assembly.GetName().Version?.ToString() ?? "1.0.0")
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = environment,
                ["host.name"] = Environment.MachineName,
                ["service.namespace"] = serviceNamespace
            });

        // Setup OpenTelemetry SDK for Tracing
        services.AddOpenTelemetry()
            .WithTracing(tracerProvider =>
            {
                tracerProvider
                    .SetResourceBuilder(resourceBuilder)
                    // Use the SamplingProbability from our centralized options
                    .SetSampler(new TraceIdRatioBasedSampler(oTelOptions.Tracing.SamplingProbability))

                    // Automatic instrumentation for ASP.NET Core requests
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        // Filter out noise from infrastructure endpoints
                        options.Filter = httpContext =>
                        {
                            var path = httpContext.Request.Path.Value ?? string.Empty;
                            return !path.Contains("/health") && !path.Contains("/swagger") && !path.Contains("/metrics");
                        };
                    })
                    // Automatic instrumentation for outgoing HTTP calls
                    .AddHttpClientInstrumentation()
                    // Automatic instrumentation for SQL Server queries
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.RecordException = true;
                    })
                    // Universal OTLP Exporter using the endpoint from options
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(oTelOptions.Tracing.OtlpEndpoint);
                        otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

        // Allow unencrypted gRPC traffic (required for Jaeger in local Docker environments)
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        return services;
    }
}