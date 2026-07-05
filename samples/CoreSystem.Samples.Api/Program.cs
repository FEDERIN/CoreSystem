using Core.Cache.DependencyInjection;
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

builder.Services.AddCoreCache(options =>
{
    builder.Configuration
        .GetSection("Cache")
        .Bind(options);

    if (!options.Redis.Enabled)
        return;

    var configurationName = options.Redis.ConfigurationName;

    var redisSection = builder.Configuration.GetSection(
    $"RedisConnections:{configurationName}");

    if (string.IsNullOrWhiteSpace(configurationName))
        throw new InvalidOperationException(
            "Cache:Redis:ConfigurationName is required when Redis is enabled.");

    options.Redis.Configuration = config =>
    {
        config.EndPoints.Add(redisSection["Host"]!);
        config.Password = redisSection["Password"];
    };
});

var app = builder.Build();

app.UseObservabilityEndpoints();
app.UseIdempotency();
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