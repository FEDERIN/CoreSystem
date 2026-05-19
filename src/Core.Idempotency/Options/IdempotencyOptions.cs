namespace Core.Idempotency.Options;

/// <summary>
/// Configuration options for the Idempotency library.
/// </summary>
public class IdempotencyOptions
{
    /// <summary>
    /// Master switch to enable or disable the idempotency logic.
    /// Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The HTTP header name used to identify unique requests.
    /// Default is "X-Idempotency-Key".
    /// </summary>
    public string HeaderName { get; set; } = "X-Idempotency-Key";

    /// <summary>
    /// The duration for which the response will be stored in the cache.
    /// Default is 24 hours.
    /// </summary>
    public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// The HTTP methods that will be intercepted by the idempotency middleware.
    /// Default is ["POST", "PUT"].
    /// </summary>
    public string[] AllowedMethods { get; set; } = ["POST", "PUT"];

    /// <summary>
    /// The storage provider to use. 
    /// Options: "Redis", "PostgreSQL".
    /// </summary>
    public string Provider { get; set; } = "Redis";

    /// <summary>
    /// The name of the OpenTelemetry meter used to report metrics.
    /// </summary>
    public string? MeterName { get; set; } = null;
}