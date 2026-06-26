namespace Core.DistributedCache.Abstractions;

public interface ICacheSerializer
{
    byte[] Serialize<T>(T value);
    T? Deserialize<T>(byte[] bytes);
    public bool RequiresHeader => false;
}