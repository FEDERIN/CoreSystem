using Core.Observability;
using Core.Idempotency;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability(
    environment: builder.Environment.EnvironmentName,
    serviceName: "Minimal.Test.Api",
    serviceNamespace: "CoreSystems");

builder.Services.AddIdempotencyProvider(builder.Configuration);

var app = builder.Build();

app.UseObservabilityEndpoints();

app.UseIdempotency();

app.MapGet("/hello", () => Results.Ok(new { message = "Tracing is active!" }));


app.MapPost("/process-order", () =>
    Results.Ok(new
    {
        id = Guid.NewGuid(),
        status = "Processed",
        processedAt = DateTime.UtcNow
    }));

app.MapDelete("/delete-item/{id}", (Guid id) =>
{
    return Results.NoContent();
})
.WithName("DeleteWithIdempotency");

try
{
    Log.Information("Starting web host with Observability and Idempotency");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}