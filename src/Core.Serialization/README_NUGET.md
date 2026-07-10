# ⚡ Core.Serialization

> **Production-ready serialization abstraction for .NET 8**

Core.Serialization (distributed as the **CoreSystem.Serialization** NuGet package) provides a unified serialization abstraction for .NET applications, allowing you to seamlessly work with **JSON**, **MessagePack**, or **Protocol Buffers** through a single API.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Serialization?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Serialization?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

---

## ✨ Features

- ✅ Unified serialization abstraction
- ✅ JSON, MessagePack and Protocol Buffers support
- ✅ Dependency Injection integration
- ✅ Configurable serializer selection
- ✅ Common exception model
- ✅ High-level serialization API
- ✅ Lightweight and reusable
- ✅ Designed for infrastructure libraries

---

## 📦 Installation

```bash
dotnet add package CoreSystem.Serialization
```

---

## 🚀 Quick Start

Register the library:

```csharp
builder.Services.AddCoreSerialization(options =>
{
    options.DefaultSerializer = SerializerType.Json;
});
```

Inject the serializer:

```csharp
public sealed class ProductService(
    IPayloadSerializer serializer)
{
}
```

Serialize an object:

```csharp
byte[] payload =
    serializer.Serialize(product);
```

Deserialize an object:

```csharp
Product? restored =
    serializer.Deserialize<Product>(payload);
```

---

## ⚙️ Configuration

Customize the serializer during registration.

```csharp
builder.Services.AddCoreSerialization(options =>
{
    options.DefaultSerializer = SerializerType.Json;

    // Configure JSON or MessagePack options here.
});
```

---

## 📚 Supported Serializers

### JSON

Recommended for:

- Human-readable payloads
- Debugging
- Maximum compatibility

---

### MessagePack

Recommended for:

- High-performance APIs
- Compact payloads
- Low latency

---

### Protocol Buffers

Recommended for:

- Cross-platform communication
- Contract-first serialization
- Compact binary payloads

> [!NOTE]
> Protocol Buffers requires serialized types to be compatible with **protobuf-net**.

```csharp
[ProtoContract]
public sealed class Product
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; } = string.Empty;
}
```

---

## ⚠️ Exception Handling

All serializer-specific exceptions are wrapped by a common exception type.

```csharp
try
{
    var payload =
        serializer.Serialize(product);
}
catch (CoreSerializationException ex)
{
    Console.WriteLine(ex.Serializer);
}
```

---

## 📊 Why Core.Serialization?

| Capability | Raw Serializer Libraries | CoreSystem.Serialization |
|------------|:------------------------:|:------------------------:|
| Unified API | ❌ | ✅ |
| Dependency Injection | ❌ | ✅ |
| JSON | ✅ | ✅ |
| MessagePack | ✅ | ✅ |
| Protocol Buffers | ✅ | ✅ |
| Configurable serializer selection | ❌ | ✅ |
| Common exception model | ❌ | ✅ |

---

## 🤝 Contributing

Issues, discussions and pull requests are welcome.

---

## 📄 License

Released under the MIT License.