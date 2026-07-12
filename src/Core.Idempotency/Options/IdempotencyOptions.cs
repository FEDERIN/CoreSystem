using Core.Idempotency.Abstractions;
using Microsoft.AspNetCore.Http;

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
    /// Gets or sets the default cache provider used by the library.
    /// </summary>
    /// <remarks>
    /// If not specified, the library selects the appropriate provider
    /// based on the registered services.
    /// </remarks>
    public IdempotencyProviderType Provider { get; set; }
        = IdempotencyProviderType.Redis;

    public RedisOptions Redis { get; } = new();

    public PostgreSqlOptions PostgreSql { get; } = new();

    /// <summary>
    /// Gets or sets an optional prefix applied to all generated cache keys.
    /// </summary>
    /// <remarks>
    /// Using an instance name allows multiple applications or environments
    /// to safely share the same Redis instance without key collisions.
    /// </remarks>
    public string? InstanceName { get; set; }

    /// <summary>
    /// The duration for which the response will be stored in the cache.
    /// Default is 30 minutes.
    /// </summary>
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// The HTTP methods that will be intercepted by the idempotency middleware.
    /// Default is ["POST", "PUT"].
    /// </summary>
    public HashSet<string> AllowedMethods { get; } = ["POST", "PUT"];


    /// <summary>
    /// HTTP status codes that will be stored and replayed for
    /// subsequent idempotent requests.
    /// </summary>
    public HashSet<int> CacheableStatusCodes { get; } =
    [
        StatusCodes.Status200OK,
        StatusCodes.Status201Created,
        StatusCodes.Status202Accepted,
        StatusCodes.Status204NoContent
    ];

    /// <summary>
    /// Adds one or more cacheable HTTP status codes.
    /// </summary>
    public void AddCacheableStatusCodes(
        params int[] statusCodes)
    {
        CacheableStatusCodes.UnionWith(statusCodes);
    }

    /// <summary>
    /// Removes one or more cacheable HTTP status codes.
    /// </summary>
    public void RemoveCacheableStatusCodes(
        params int[] statusCodes)
    {
        foreach (var statusCode in statusCodes)
        {
            CacheableStatusCodes.Remove(statusCode);
        }
    }

    /// <summary>
    /// Adds one or more allowed HTTP methods.
    /// </summary>
    public void AddAllowedMethods(
        params string[] methods)
    {
        AllowedMethods.UnionWith(
            methods.Select(m => m.ToUpperInvariant()));
    }

    /// <summary>
    /// Removes one or more allowed HTTP methods.
    /// </summary>
    public void RemoveAllowedMethods(
        params string[] methods)
    {
        foreach (var method in methods)
        {
            AllowedMethods.Remove(
                method.ToUpperInvariant());
        }
    }
}