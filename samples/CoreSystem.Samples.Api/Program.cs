using Core.DistributedCache.Abstractions;
using Core.DistributedCache.DependencyInjection;
using Core.DistributedCache.Options;
using Core.Idempotency;
using Core.Observability;
using CoreSystem.Samples.Core.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IMyService, MyService>();

builder.AddObservability(
    environment: builder.Environment.EnvironmentName,
    serviceName: "Minimal.Test.Api",
    serviceNamespace: "CoreSystems");

builder.Services.AddIdempotencyProvider(builder.Configuration);

builder.Services.AddCoreDistributedCache(options =>
{
    builder.Configuration.GetSection("DistributedCache").Bind(options);

    if (options.DefaultProvider == CacheProviderType.Redis)
    {
        options.Redis = new RedisOptions
        {
            Enabled = true,
            Configuration = config =>
            {
                config.EndPoints.Add("localhost:6379");
                config.Password = "foobared";
            }
        };
    }
});

var app = builder.Build();

app.UseObservabilityEndpoints();
app.UseIdempotency();
app.UseCoreDistributedCache();


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