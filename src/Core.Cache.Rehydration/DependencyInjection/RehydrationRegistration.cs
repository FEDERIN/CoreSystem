using Core.Cache.Options;
using Core.Cache.Rehydration.Abstractions;
using Core.Cache.Rehydration.Background;
using Core.Cache.Rehydration.Memory;
using Core.Cache.Rehydration.Options;
using Core.Cache.Rehydration.Primary;
using Core.Cache.Rehydration.Services;
using Core.Cache.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.Rehydration.DependencyInjection;

public static class RehydrationRegistration
{
    public static IServiceCollection AddCoreCacheRehydration(
        this IServiceCollection services,
        Action<RehydrationOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var cacheOptions = GetCacheOptions(services);

        if (!cacheOptions.Enabled)
        {
            return services;
        }

        EnsurePrimaryRegistered(services);

        var rehydrationOptions = BuildOptions(configure);

        services.AddSingleton(rehydrationOptions);

        if (!rehydrationOptions.Enabled)
            return services;

        services.AddSingleton<IRehydrationSource,
            MemoryRehydrationSource>();

        services.AddSingleton<IRehydrationTarget,
            PrimaryRehydrationTarget>();

        services.AddSingleton<ICacheRehydrator,
            CacheRehydrator>();

        services.AddSingleton<IRehydrationService,
            RehydrationService>();

        services.AddHostedService<
            RehydrationBackgroundService>();

        return services;
    }

    private static CacheOptions GetCacheOptions(
    IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(
            x => x.ServiceType == typeof(CacheOptions));

        if (descriptor?.ImplementationInstance is not CacheOptions options)
        {
            throw new InvalidOperationException(
                RehydrationMessages.CacheRegistrationRequired);
        }

        return options;
    }

    private static RehydrationOptions BuildOptions(
        Action<RehydrationOptions> configure)
    {
        var options = new RehydrationOptions();

        configure(options);

        return options;
    }

    private static void EnsurePrimaryRegistered(
    IServiceCollection services)
    {
        if (services.All(
            d => d.ServiceType != typeof(IExternalCacheStorage)))
        {
            throw new InvalidOperationException(
                RehydrationMessages.PrimaryRegistrationRequired);
        }
    }
}