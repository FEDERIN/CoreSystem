namespace Core.DistributedCache.Abstractions;

public interface ICacheSerializer
{
    string Serialize<T>(T value);
    T? Deserialize<T>(string value);
}