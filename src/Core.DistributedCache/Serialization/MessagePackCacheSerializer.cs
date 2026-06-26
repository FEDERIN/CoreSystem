using Core.DistributedCache.Abstractions;
using MessagePack;
using MessagePack.Resolvers;


namespace Core.DistributedCache.Serialization;

public class MessagePackCacheSerializer : ICacheSerializer
{
    private readonly MessagePackSerializerOptions _options =
        MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);

    public bool RequiresHeader => true;

    public byte[] Serialize<T>(T value) => MessagePackSerializer.Serialize(value, _options);

    public T? Deserialize<T>(byte[] bytes) => MessagePackSerializer.Deserialize<T>(bytes, _options);
}