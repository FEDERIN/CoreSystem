using Core.DistributedCache.Serialization;
using FluentAssertions;
using MessagePack;

namespace Core.DistributedCache.UnitTests.Serialization;

public sealed class MessagePackCacheSerializerTests
{
    private readonly MessagePackCacheSerializer _serializer = new();

    [Fact]
    public void Serialize_String_ShouldWork()
    {
        var serializer = new MessagePackCacheSerializer();

        var bytes = serializer.Serialize("Hello");

        var result = serializer.Deserialize<string>(bytes);

        result.Should().Be("Hello");
    }

    [Fact]
    public void SerializeDeserialize_Object_ShouldRoundTrip()
    {
        // Arrange
        var expected = new CustomerDto
        {
            Id = 1,
            Name = "Juan"
        };

        // Act
        var bytes = _serializer.Serialize(expected);

        var result = _serializer.Deserialize<CustomerDto>(bytes);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SerializeDeserialize_Primitive_ShouldRoundTrip()
    {
        // Arrange
        const string expected = "Hello MessagePack";

        // Act
        var bytes = _serializer.Serialize(expected);

        var result = _serializer.Deserialize<string>(bytes);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void SerializeDeserialize_List_ShouldRoundTrip()
    {
        // Arrange
        var expected = new List<CustomerDto>
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

        var result = _serializer.Deserialize<List<CustomerDto>>(bytes);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SerializeDeserialize_Dictionary_ShouldRoundTrip()
    {
        // Arrange
        var expected = new Dictionary<string, CustomerDto>
        {
            ["1"] = new()
            {
                Id = 1,
                Name = "Juan"
            },
            ["2"] = new()
            {
                Id = 2,
                Name = "Pedro"
            }
        };

        // Act
        var bytes = _serializer.Serialize(expected);

        var result =
            _serializer.Deserialize<Dictionary<string, CustomerDto>>(bytes);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void RequiresHeader_ShouldBeTrue()
    {
        // Assert
        _serializer.RequiresHeader.Should().BeTrue();
    }
}


[MessagePackObject(AllowPrivate = true)]
internal sealed class CustomerDto
{
    [Key(0)]
    public int Id { get; set; }

    [Key(1)]
    public string Name { get; set; } = string.Empty;
}