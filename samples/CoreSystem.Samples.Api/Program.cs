using Core.Cache.DependencyInjection;
using Core.Idempotency;
using Core.Idempotency.Abstractions;
using Core.Idempotency.DependencyInjection;
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

var applyIdempotency = false;

builder.Services.AddCoreIdempotency(options =>
{
    builder.Configuration
        .GetSection("Idempotency")
        .Bind(options);

    if (!options.Enabled)
        return;

    applyIdempotency = true;

    if (options.Provider == IdempotencyProviderType.Redis)
    {
        if(options.Redis == null)
            throw new InvalidOperationException(
                "Idempotency:Redis is required when Redis is enabled.");

        var configurationName = options.Redis.Connection;

        if (string.IsNullOrWhiteSpace(configurationName))
            throw new InvalidOperationException(
                "Idempotency:Redis:ConfigurationName is required when Redis is enabled.");

        var redisSection = builder.Configuration.GetSection(
            $"RedisConnections:{configurationName}") ?? throw new InvalidOperationException(
                $"Redis connection configuration '{configurationName}' not found.");

        options.Redis.Configuration = config =>
        {
            config.EndPoints.Add(redisSection["Host"]!);
            config.Password = redisSection["Password"];
        };
    }
});

builder.Services.AddCoreCache(options =>
{
    builder.Configuration
        .GetSection("Cache")
        .Bind(options);

    if (!options.Redis.Enabled)
        return;

    var configurationName = options.Redis.Connection;

    if (string.IsNullOrWhiteSpace(configurationName))
        throw new InvalidOperationException(
            "Cache:Redis:ConfigurationName is required when Redis is enabled.");

    var redisSection = builder.Configuration.GetSection(
        $"RedisConnections:{configurationName}") ?? throw new InvalidOperationException(
            $"Redis connection configuration '{configurationName}' not found.");

    options.Redis.Configuration = config =>
    {
        config.EndPoints.Add(redisSection["Host"]!);
        config.Password = redisSection["Password"];
    };

    options.InstanceName = "CoreSystem:App01";
});

var app = builder.Build();

app.UseObservabilityEndpoints();

if (applyIdempotency)
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