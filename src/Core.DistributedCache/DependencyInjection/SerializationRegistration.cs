using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Serialization;
using Core.DistributedCache.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.DependencyInjection;

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