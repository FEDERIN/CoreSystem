using Core.Observability;
using Core.Idempotency;
using Serilog;
using Core.Idempotency.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddObservability(
    environment: builder.Environment.EnvironmentName,
    serviceName: "Minimal.Test.Api",
    serviceNamespace: "CoreSystems");


var idempotencyOptions = new IdempotencyOptions();
builder.Configuration.GetSection("Idempotency").Bind(idempotencyOptions);

// Logic to decide provider based on configuration
if (idempotencyOptions.Provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
{
    var postgresConn = builder.Configuration.GetConnectionString("PostgresIdempotency");
    builder.Services.AddPostgresIdempotency(postgresConn ?? string.Empty, options =>
    {
        builder.Configuration.GetSection("Idempotency").Bind(options);
    });
}
else
{
    var redisConn = builder.Configuration.GetConnectionString("RedisCloud");
    builder.Services.AddRedisIdempotency(redisConn ?? string.Empty, options =>
    {
        builder.Configuration.GetSection("Idempotency").Bind(options);
    });
}

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