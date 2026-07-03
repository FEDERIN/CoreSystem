namespace Core.Cache.Attributes;

/// <summary>
/// Indicates that the result of an HTTP endpoint should be cached.
/// </summary>
/// <remarks>
/// When applied to an endpoint, the middleware automatically stores
/// successful responses and serves subsequent requests directly from the cache
/// until the configured expiration period elapses.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class CacheableAttribute(int expirationSeconds = 0, string tag = "") : Attribute
{
    public string? Tag { get; } = !string.IsNullOrEmpty(tag) ? tag : null;
    public int? ExpirationSeconds { get; } = expirationSeconds > 0 ? expirationSeconds : null;
}