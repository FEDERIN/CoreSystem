using Core.Observability;

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

app.Run();