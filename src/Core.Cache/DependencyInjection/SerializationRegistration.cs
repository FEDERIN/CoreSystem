using Core.Cache.Abstractions;
using Core.Cache.Serialization;
using Core.Cache.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

internal static class SerializationRegistration
{
    public static IServiceCollection AddCacheSerialization(
        this IServiceCollection services)
    {
        services.AddSingleton<JsonCacheSerializer>();
        services.AddSingleton<MessagePackCacheSerializer>();
        services.AddSingleton<ProtobufCacheSerializer>();
        services.AddSingleton<ICacheSerializerFactory, CacheSerializerFactory>();

        return services;
    }
}