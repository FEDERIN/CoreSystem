namespace Core.Resilience.Abstractions;

/// <summary>
/// Represents the predefined resilience pipelines available in the system.
/// </summary>
/// <remarks>
/// Pipeline types are used to resolve an <see cref="IResiliencePipeline"/>
/// instance through <see cref="IResiliencePipelineProvider"/>. Each value
/// identifies a logical execution pipeline that can be configured with
/// different resilience strategies such as retries, circuit breakers,
/// and timeouts.
/// </remarks>
public enum PipelineType
{
    /// <summary>
    /// The default pipeline used when no specific pipeline is requested.
    /// </summary>
    Default,

    /// <summary>
    /// Pipeline intended for Redis operations.
    /// </summary>
    Redis,

    /// <summary>
    /// Pipeline intended for SQL database operations.
    /// </summary>
    Sql,

    /// <summary>
    /// Pipeline intended for HTTP requests.
    /// </summary>
    Http,

    /// <summary>
    /// Pipeline intended for messaging systems such as queues or brokers.
    /// </summary>
    Messaging
}