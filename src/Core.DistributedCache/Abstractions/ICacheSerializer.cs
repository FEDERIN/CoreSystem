namespace Core.DistributedCache.Abstractions;

internal interface ICacheSerializer
{
    byte[] Serialize<T>(T value);
    T? Deserialize<T>(byte[] bytes);
    bool RequiresHeader => false;
}