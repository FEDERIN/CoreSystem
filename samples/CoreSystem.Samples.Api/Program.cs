using Core.Cache.DependencyInjection;
using Core.Cache.Options;
using Core.Idempotency.Abstractions;
using Core.Idempotency.DependencyInjection;
using Core.Observability;
using Core.Resilience.DependencyInjection;
using CoreSystem.Samples.Core.Services;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IMyService, MyService>();

var applyIdempotency = false;


var redisSection = builder.Configuration.GetSection("RedisConnections:MainRedis");

builder.Services.AddCoreIdempotency(options =>
{
    builder.Configuration
        .GetSection("Core:Idempotency")
        .Bind(options);

    if (!options.Enabled)
        return;

    applyIdempotency = true;

    if (options.Provider == IdempotencyProviderType.Redis)
    {
        var redisConfiguration = CreateRedisConfiguration(redisSection);

        options.Redis.Configuration = redisConfiguration;
    }
});

builder.Services.AddCoreCache(options =>
{
    builder.Configuration
        .GetSection("Cache")
        .Bind(options);

    if (!options.Redis.Enabled)
        return;

    var redisConfiguration = CreateRedisConfiguration(redisSection);

    options.Redis.Configuration = redisConfiguration;

    options.InstanceName = "CoreSystem:App01";
});

builder.AddObservability(
    environment: builder.Environment.EnvironmentName,
    serviceName: "Minimal.Test.Api",
    serviceNamespace: "CoreSystems");

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

static Action<ConfigurationOptions> CreateRedisConfiguration(IConfigurationSection redisSection)
{
    ValidateRedisConnection(redisSection);

    return config =>
    {
        config.EndPoints.Add(redisSection["Host"]!);
        config.Password = redisSection["Password"];
    };
}

static void ValidateRedisConnection(IConfigurationSection redisSection)
{
    if (!redisSection.Exists())
    {
        throw new InvalidOperationException(
            "The configuration section 'RedisConnections:MainRedis' was not found.");
    }

    var host = redisSection["Host"];

    if (string.IsNullOrWhiteSpace(host))
    {
        throw new InvalidOperationException(
            "RedisConnections:Default:Host is required.");
    }

    var password = redisSection["Password"];

    if (string.IsNullOrWhiteSpace(password))
    {
        throw new InvalidOperationException(
            "RedisConnections:Default:Password is required.");
    }
}