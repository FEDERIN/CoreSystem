using Core.DistributedCache.Options;
using Core.DistributedCache.Serialization;
using Core.DistributedCache.Storage;
using Core.DistributedCache.Storage.Redis;
using FluentAssertions;
using MessagePack;

namespace Core.DistributedCache.UnitTests.Serialization;

public class PayloadSerializerTests
{
    [Fact]
    public void Serialize_ThenDeserializeTypeJson_ShouldRoundTrip()
    {
        // Arrange
        var serializer = CreateSerializer(SerializerType.Json);

        var expected = new CustomerDto
        {
            Id = 10,
            Name = "Juan"
        };

        // Act
        var payload = serializer.Serialize(expected);

        var result = serializer.Deserialize<CustomerDto>(payload);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Serialize_ThenDeserializeTypeProtobuf_ShouldRoundTrip()
    {
        // Arrange
        var serializer = CreateSerializer(SerializerType.Protobuf);

        var expected = new ProtoCustomerDto
        {
            Id = 10,
            Name = "Juan"
        };

        // Act
        var payload = serializer.Serialize(expected);

        var result = serializer.Deserialize<ProtoCustomerDto>(payload);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Serialize_ThenDeserializeTypeMessagePack_ShouldRoundTrip()
    {
        // Arrange
        var serializer = CreateSerializer(SerializerType.MessagePack);

        var expected = new Customer
        {
            Id = 10,
            Name = "Juan"
        };

        // Act
        var payload = serializer.Serialize(expected);

        var result = serializer.Deserialize<Customer>(payload);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Serialize_Json_ShouldNotAddSerializerHeader()
    {
        // Arrange
        var serializer = CreateSerializer(SerializerType.Json);

        // Act
        var payload = serializer.Serialize("Hello");

        // Assert
        payload.Should().NotBeEmpty();

        Enum.IsDefined(typeof(SerializerType), payload[0])
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Serialize_MessagePack_ShouldAddSerializerHeader()
    {
        // Arrange
        var serializer = CreateSerializer(SerializerType.MessagePack);

        // Act
        var payload = serializer.Serialize("Hello");

        // Assert
        payload[0].Should().Be((byte)SerializerType.MessagePack);
    }

    [Fact]
    public void Serialize_Protobuf_ShouldAddSerializerHeader()
    {
        // Arrange
        var serializer = CreateSerializer(SerializerType.Protobuf);

        var value = new ProtoCustomerDto
        {
            Id = 1,
            Name = "Juan"
        };

        // Act
        var payload = serializer.Serialize(value);

        // Assert
        payload[0].Should().Be((byte)SerializerType.Protobuf);
    }

    [Fact]
    public void Deserialize_EmptyPayload_ShouldReturnDefault()
    {
        // Arrange
        var serializer = CreateSerializer(SerializerType.Json);

        // Act
        var result = serializer.Deserialize<CustomerDto>(ReadOnlyMemory<byte>.Empty);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Deserialize_JsonPayloadWithoutHeader_ShouldDeserializeSuccessfully()
    {
        // Arrange
        var jsonSerializer = new JsonCacheSerializer();

        var expected = new CustomerDto
        {
            Id = 7,
            Name = "Pedro"
        };

        var payload = jsonSerializer.Serialize(expected);

        var serializer = CreateSerializer(SerializerType.MessagePack);

        // Act
        var result = serializer.Deserialize<CustomerDto>(payload);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    private static PayloadSerializer CreateSerializer(
        SerializerType serializerType)
    {
        var options = new CacheOptions
        {
            SerializerType = serializerType
        };

        var factory = new CacheSerializerFactory(
            new JsonCacheSerializer(),
            new MessagePackCacheSerializer(),
            new ProtobufCacheSerializer());

        return new PayloadSerializer(options, factory);
    }

    private sealed class CustomerDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;
    }

    [ProtoBuf.ProtoContract]
    private sealed class ProtoCustomerDto
    {
        [ProtoBuf.ProtoMember(1)]
        public int Id { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; } = string.Empty;
    }

    [MessagePackObject(AllowPrivate = true)]
    internal sealed class Customer
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; } = string.Empty;
    }
}