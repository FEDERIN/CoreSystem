using Core.Serialization.Abstractions;
using Core.Serialization.Json;
using Core.Serialization.MessagePack;
using Core.Serialization.Protobuf;
using Microsoft.Extensions.DependencyInjection;


namespace Core.Serialization.DependencyInjection;

/// <summary>
/// Provides extension methods for registering Core.Serialization services.
/// </summary>
public static class SerializationRegistration
{
    /// <summary>
    /// Registers the Core.Serialization services and serializer implementations.
    /// </summary>
    /// <param name="services">
    /// The service collection.
    /// </param>
    /// <param name="configure">
    /// Optional delegate used to configure serialization options.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/>.
    /// </returns>
    /// <remarks>
    /// This method registers:
    /// <list type="bullet">
    /// <item><description>JSON serializer.</description></item>
    /// <item><description>MessagePack serializer.</description></item>
    /// <item><description>Protocol Buffers serializer.</description></item>
    /// <item><description><see cref="ISerializerFactory"/>.</description></item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddCoreSerialization(
        this IServiceCollection services,
        Action<SerializationOptions>? configure = null)
    {
        var options = new SerializationOptions();

        configure?.Invoke(options);

        services.AddSingleton(options);

        services.AddSingleton(sp =>
        {
            var opts = sp.GetRequiredService<SerializationOptions>();
            return new JsonPayloadSerializer(opts.JsonOptions);
        });

        services.AddSingleton(sp =>
        {
            var opts = sp.GetRequiredService<SerializationOptions>();
            return new MessagePackPayloadSerializer(opts.MessagePackOptions);
        });

        services.AddSingleton<ProtobufPayloadSerializer>();

        services.AddSingleton<ISerializerFactory, SerializerFactory>();
        services.AddSingleton<IPayloadSerializer, PayloadSerializer>();
        return services;
    }
}