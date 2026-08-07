using Core.Idempotency.Abstractions;
using Core.Idempotency.DependencyInjection;
using Core.Idempotency.Options;
using CoreSystem.Samples.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSystem.Samples.Infrastructure.DependencyInjection;

internal static class IdempotencyExtension
{
    public static IServiceCollection AddIdempotencyInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("Core:Idempotency");

        if (!section.Exists())
        {
            return services;
        }

        var options = new IdempotencyOptions();
        section.Bind(options);

        if (!options.Enabled)
            return services;

        section.ReplaceIfConfigured(
            "AllowedMethods",
            options.AllowedMethods,
            options.AddAllowedMethods);

        section.ReplaceIfConfigured(
            "CacheableStatusCodes",
            options.CacheableStatusCodes,
            options.AddCacheableStatusCodes);

        if (options.AllowedMethods.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one HTTP method must be configured for Core:Idempotency:AllowedMethods.");
        }

        //if (options.Provider == IdempotencyProviderType.Redis)
        //{
        //    ConfigureRedis(options, configuration);
        //}

        services.AddCoreIdempotency(_ => _.CopyFrom(options));

        return services;
    }

    //private static void ConfigureRedis(
    //    IdempotencyOptions options,
    //    IConfiguration configuration)
    //{
    //    options.Redis.Configuration =
    //        RedisConfigurationFactory.Create(
    //            configuration,
    //            "MainRedis");
    //}
}