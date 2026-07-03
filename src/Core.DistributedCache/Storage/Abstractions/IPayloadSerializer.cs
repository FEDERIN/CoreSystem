namespace Core.DistributedCache.Storage.Abstractions;

internal interface IPayloadSerializer
{
    byte[] Serialize<T>(T value);

    T? Deserialize<T>(ReadOnlyMemory<byte> payload);
}