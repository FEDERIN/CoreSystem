using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Core.Observability.Extensions;

public static class HealthCheckEndpointsExtensions
{
    public static WebApplication MapCoreHealthEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = async (ctx, _) =>
            {
                ctx.Response.ContentType = "application/json; charset=utf-8";
                await ctx.Response.WriteAsync("""{"status":"Healthy"}""");
            }
        }).DisableRateLimiting();

        app.MapHealthChecks("/ready", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = WriteHealthResponse
        }).DisableRateLimiting();

        return app;
    }

    private static Task WriteHealthResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                durationMs = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                data = e.Value.Data
            })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}