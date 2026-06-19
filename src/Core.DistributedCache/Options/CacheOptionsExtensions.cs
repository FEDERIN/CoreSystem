
namespace Core.DistributedCache.Options;

public static class CacheOptionsExtensions
{
    public static CacheOptions WithDefaultExpiration(this CacheOptions options, TimeSpan expiration)
    {
        options.DefaultExpiration = expiration;
        return options;
    }

    public static CacheOptions WithMaxCacheableSize(this CacheOptions options, long sizeInBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sizeInBytes);
        options.MaxCacheableSize = sizeInBytes;
        return options;
    }
}