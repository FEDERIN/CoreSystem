using Core.Observability.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using System.Diagnostics;

namespace Core.Observability.Extensions;

/// <summary>
/// Extension methods to configure Serilog with Console and OpenTelemetry sinks.
/// </summary>
internal static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilogConfiguration(
        this WebApplicationBuilder builder,
        string environment,
        string serviceName)
    {
        // Ensure builder is not null before proceeding
        ArgumentNullException.ThrowIfNull(builder);

        var oTelOptions = new OpenTelemetryOptions();
        var section = builder.Configuration.GetSection(OpenTelemetryOptions.SectionName);

        // If the section exists, bind it to our options class
        if (section.Exists())
        {
            section.Bind(oTelOptions);
        }

        builder.Host.UseSerilog((context, loggerConfig) =>
        {
            // Base configuration: Structured logging for Console
            loggerConfig
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("machine.name", Environment.MachineName)
                .Enrich.WithThreadId()
                .Enrich.WithProperty("environment", environment ?? "unknown")
                .Enrich.WithProperty("service.name", serviceName ?? "unknown-service")
                .WriteTo.Console();

            // Configure OpenTelemetry Sink if enabled
            if (oTelOptions.Logging != null && oTelOptions.Logging.Enabled)
            {
                var endpoint = oTelOptions.Logging.OtlpEndpoint;

                // Resilience check: Validate the OTLP endpoint before attaching the sink
                if (!string.IsNullOrWhiteSpace(endpoint) &&
                    Uri.TryCreate(endpoint, UriKind.Absolute, out _))
                {
                    loggerConfig.WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = endpoint;
                        options.Protocol = OtlpProtocol.Grpc;
                        options.ResourceAttributes = new Dictionary<string, object>
                        {
                            ["service.name"] = serviceName ?? "unknown-service",
                            ["deployment.environment"] = environment ?? "unknown"
                        };
                    });
                }
                else
                {
                    // Fail-safe: If the endpoint is invalid, we still have Console logging active
                    Trace.WriteLine($"[Observability] Serilog OTLP Endpoint '{endpoint}' is invalid or empty. OTLP sink skipped.");
                }
            }
        });

        return builder;
    }
}