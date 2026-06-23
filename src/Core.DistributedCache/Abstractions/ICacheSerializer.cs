namespace Core.DistributedCache.Abstractions;

internal interface ICacheSerializer
{
    string Serialize<T>(T value);
    T? Deserialize<T>(string value);
}