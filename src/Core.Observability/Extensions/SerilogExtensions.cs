using Core.Observability.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Core.Observability.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilogConfiguration(
        this WebApplicationBuilder builder,
        string environment,
        string serviceName)
    {
        var oTelOptions = new OpenTelemetryOptions();
        builder.Configuration.GetSection(OpenTelemetryOptions.SectionName).Bind(oTelOptions);

        builder.Host.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("machine.name", Environment.MachineName)
                .Enrich.WithThreadId()
                .Enrich.WithProperty("environment", environment)
                .Enrich.WithProperty("service.name", serviceName)
                .WriteTo.Console();

            if (oTelOptions.Logging.Enabled)
            {
                loggerConfig.WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = oTelOptions.Logging.OtlpEndpoint;
                    options.Protocol = OtlpProtocol.Grpc;
                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = serviceName,
                        ["deployment.environment"] = environment
                    };
                });
            }
        });

        return builder;
    }
}