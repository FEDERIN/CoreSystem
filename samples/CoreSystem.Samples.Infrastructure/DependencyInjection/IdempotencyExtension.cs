using Core.Idempotency.DependencyInjection;
using Core.Idempotency.Options;
using Core.Idempotency.Redis.DependencyInjection;
using Core.Idempotency.PostgreSql.DependencyInjection;
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
        {
            return services;
        }

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

        services.AddCoreIdempotency(
            _ => _.CopyFrom(options));

        ConfigureProvider(
            services,
            configuration,
            section);

        return services;
    }

    private static void ConfigureProvider(
        IServiceCollection services,
        IConfiguration configuration,
        IConfigurationSection section)
    {
        var provider =
            section.GetValue<string>("Provider");

        switch (provider)
        {
            case "Redis":
                services.AddCoreIdempotencyRedis(options =>
                {
                    options.Configuration =
                        RedisConfigurationFactory.Create(
                            configuration,
                            "MainRedis");
                });
                break;

            case "PostgreSql":
                services.AddCoreIdempotencyPostgreSql(options =>
                {
                    options.ConnectionString =
                        PostgreSqlConfigurationFactory.Create(
                            configuration,
                            "MainPostgreSql");
                });
                break;
        }
    }
}