using StackExchange.Redis;

namespace Core.Idempotency.Options;

/// <summary>
/// Configuration options for the Redis provider.
/// </summary>
public sealed class RedisOptions
{
    /// <summary>
    /// Delegate used to configure the Redis connection.
    /// </summary>
    public Action<ConfigurationOptions>? Configuration { get; set; }

    /// <summary>
    /// Copies the configuration from another RedisOptions instance.
    /// </summary>
    /// <param name="source"></param>
    public void CopyFrom(RedisOptions source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Configuration = source.Configuration;
    }
}