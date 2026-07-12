using StackExchange.Redis;

namespace Core.Idempotency.Options;

public sealed class RedisOptions
{
    /// <summary>
    /// Delegate used to configure the Redis connection.
    /// </summary>
    public Action<ConfigurationOptions>? Configuration { get; set; }

    /// <summary>
    /// Gets or sets the name of the configuration section used to configure Redis.
    /// </summary>
    public string? Connection { get; set; }
}