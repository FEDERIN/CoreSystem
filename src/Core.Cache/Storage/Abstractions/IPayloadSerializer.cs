namespace Core.Cache.Storage.Abstractions;

internal interface IPayloadSerializer
{
    byte[] Serialize<T>(T value);

    T? Deserialize<T>(ReadOnlyMemory<byte> payload);
}