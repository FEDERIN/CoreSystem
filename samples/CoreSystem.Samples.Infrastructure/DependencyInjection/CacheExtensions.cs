using Core.Cache.DependencyInjection;
using Core.Cache.Options;
using Core.Cache.Redis.DependencyInjection;
using Core.Cache.Rehydration.DependencyInjection;
using Core.Cache.Rehydration.Options;
using CoreSystem.Samples.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSystem.Samples.Infrastructure.DependencyInjection;

internal static class CacheExtensions
{
    public static IServiceCollection AddCacheInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cacheSection =
            configuration.GetSection("Core:Cache");

        var cacheOptions = new CacheOptions();

        cacheSection.Bind(cacheOptions);

        services.AddCoreCache(
            _ => _.CopyFrom(cacheOptions));

        if (!cacheOptions.Enabled)
        {
            return services;
        }

        var providerConfigured = ConfigureProvider(
            services,
            configuration,
            cacheSection);

        if (!providerConfigured)
        {
            return services;
        }

        var rehydrationSection =
            configuration.GetSection("Core:Rehydration");

        var rehydrationOptions = new RehydrationOptions();

        rehydrationSection.Bind(rehydrationOptions);

        if (rehydrationOptions.Enabled)
        {
            services.AddCoreCacheRehydration(
                _ => _.CopyFrom(rehydrationOptions));
        }

        return services;
    }

    private static bool ConfigureProvider(
        IServiceCollection services,
        IConfiguration configuration,
        IConfigurationSection section)
    {
        var provider =
            section.GetValue<string>("Provider");

        if (string.IsNullOrWhiteSpace(provider))
        {
            return false;
        }

        switch (provider)
        {
            case "Redis":
                services.AddCoreCacheRedis(options =>
                {
                    options.Configuration =
                        RedisConfigurationFactory.Create(
                            configuration,
                            "MainRedis");
                });

                return true;

            default:
                throw new InvalidOperationException(
                    $"Unsupported cache provider: {provider}");
        }
    }
}