using Core.Cache.Serialization;
using FluentAssertions;
using ProtoBuf;

namespace Core.Cache.UnitTests.Serialization;

public sealed class ProtobufCacheSerializerTests
{
    private readonly ProtobufCacheSerializer _serializer = new();

    [Fact]
    public void RequiresHeader_ShouldBeTrue()
    {
        // Assert
        _serializer.RequiresHeader.Should().BeTrue();
    }

    [Fact]
    public void SerializeDeserialize_Object_ShouldRoundTrip()
    {
        // Arrange
        var expected = new ProtoCustomerDto
        {
            Id = 1,
            Name = "Juan"
        };

        // Act
        var bytes = _serializer.Serialize(expected);

        var result = _serializer.Deserialize<ProtoCustomerDto>(bytes);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SerializeDeserialize_List_ShouldRoundTrip()
    {
        // Arrange
        var expected = new List<ProtoCustomerDto>
        {
            new()
            {
                Id = 1,
                Name = "Juan"
            },
            new()
            {
                Id = 2,
                Name = "Pedro"
            }
        };

        // Act
        var bytes = _serializer.Serialize(expected);

        var result = _serializer.Deserialize<List<ProtoCustomerDto>>(bytes);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SerializeDeserialize_Primitive_ShouldRoundTrip()
    {
        // Arrange
        const int expected = 12345;

        // Act
        var bytes = _serializer.Serialize(expected);

        var result = _serializer.Deserialize<int>(bytes);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Deserialize_EmptyArray_ShouldReturnDefault()
    {
        // Arrange
        byte[] bytes = [];

        // Act
        var result = _serializer.Deserialize<ProtoCustomerDto>(bytes);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Serialize_ShouldReturnBytes()
    {
        // Arrange
        var customer = new ProtoCustomerDto
        {
            Id = 10,
            Name = "Pedro"
        };

        // Act
        var bytes = _serializer.Serialize(customer);

        // Assert
        bytes.Should().NotBeNull();
        bytes.Should().NotBeEmpty();
    }

    [ProtoContract]
    private sealed class ProtoCustomerDto
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; } = string.Empty;
    }
}