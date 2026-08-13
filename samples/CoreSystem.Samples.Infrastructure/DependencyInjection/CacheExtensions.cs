using Core.Cache.DependencyInjection;
using Core.Cache.Options;
using Core.Cache.Redis.DependencyInjection;
using Core.Cache.Rehydration.DependencyInjection;
using CoreSystem.Samples.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSystem.Samples.Infrastructure.DependencyInjection;

internal static class CacheExtensions
{
    public static IServiceCollection AddCacheInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Core:Cache");

        var options = new CacheOptions();

        section.Bind(options);

        services.AddCoreCache(
            _ => _.CopyFrom(options));

        if (options.Enabled == false)
        {
            return services;
        }

        var providerConfigured = ConfigureProvider(
            services,
            configuration,
            section);

        if (providerConfigured)
        {
            services.AddCoreCacheRehydration();
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