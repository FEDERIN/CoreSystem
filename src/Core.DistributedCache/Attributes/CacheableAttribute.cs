namespace Core.DistributedCache.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CacheableAttribute(int expirationSeconds = 0, string tag = "") : Attribute
{
    public string? Tag { get; } = !string.IsNullOrEmpty(tag) ? tag :  null ;
    public int? ExpirationSeconds { get; } = expirationSeconds > 0 ? expirationSeconds: null;
}