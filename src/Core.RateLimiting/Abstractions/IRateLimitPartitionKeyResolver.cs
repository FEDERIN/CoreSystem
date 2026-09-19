using Microsoft.AspNetCore.Http;

namespace Core.RateLimiting.Abstractions;

/// <summary>
/// Defines a contract for resolving the partition key used by a rate limiter
/// for an incoming HTTP request.
/// </summary>
public interface IRateLimitPartitionKeyResolver
{
    /// <summary>
    /// Resolves the partition key for the specified HTTP request.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="HttpContext"/>.
    /// </param>
    /// <returns>
    /// A string representing the partition key used to identify the rate-limiting
    /// subject for the request.
    /// </returns>
    string Resolve(HttpContext context);
}
