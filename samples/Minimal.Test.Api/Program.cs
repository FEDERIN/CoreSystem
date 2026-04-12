using Core.Observability;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Register everything in one go
builder.AddObservability(
    environment: builder.Environment.EnvironmentName,
    serviceName: "Minimal.Test.Api",
    serviceNamespace: "CoreSystems"
);

var app = builder.Build();

// Enable the endpoints and request logging
app.UseObservabilityEndpoints();

app.MapGet("/hello", () => Results.Ok(new { message = "Tracing is active!" }));

try
{
    Log.Information("Starting web host");
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