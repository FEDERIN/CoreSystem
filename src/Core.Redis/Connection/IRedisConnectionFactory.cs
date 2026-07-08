using StackExchange.Redis;

namespace Core.Redis.Connection;

/// <summary>
/// Defines a factory for creating Redis connection multiplexers.
/// </summary>
/// <remarks>
/// This abstraction centralizes the creation and configuration of
/// <see cref="IConnectionMultiplexer"/> instances, ensuring consistent
/// connection settings across the application.
/// </remarks>
public interface IRedisConnectionFactory
{
    /// <summary>
    /// Creates a new <see cref="IConnectionMultiplexer"/> instance using the
    /// specified Redis configuration.
    /// </summary>
    /// <param name="configure">
    /// An optional delegate used to configure the
    /// <see cref="ConfigurationOptions"/> before establishing the connection.
    /// </param>
    /// <returns>
    /// A configured <see cref="IConnectionMultiplexer"/> instance.
    /// </returns>
    IConnectionMultiplexer Create(
        Action<ConfigurationOptions>? configure);
}