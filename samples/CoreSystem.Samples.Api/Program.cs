using Core.Cache.DependencyInjection;
using Core.Idempotency.DependencyInjection;
using Core.Observability;
using CoreSystem.Samples.Core.Interfaces;
using CoreSystem.Samples.Core.Services;
using CoreSystem.Samples.Infrastructure.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddInfrastructure(builder.Configuration);


builder.AddObservability(
    environment: builder.Environment.EnvironmentName,
    serviceName: "Minimal.Test.Api",
    serviceNamespace: "CoreSystems");

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseObservabilityEndpoints();
app.UseCoreIdempotency();
app.UseCoreCache();

try
{
    app.MapControllers();
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