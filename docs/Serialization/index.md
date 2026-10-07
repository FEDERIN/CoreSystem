# CoreSystem.Serialization

> **Production-ready serialization abstraction for .NET 8.**

CoreSystem.Serialization provides a unified serialization abstraction for
.NET applications, allowing you to work with **JSON**, **MessagePack**, or
**Protocol Buffers** through a single API.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Serialization?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Serialization?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=for-the-badge)

---

## Features

- Unified serialization abstraction (`ISerializerFactory`)
- JSON, MessagePack, and Protocol Buffers support
- Dependency Injection integration
- Configurable serializer selection
- Common exception model (`CoreSerializationException`)
- Lightweight and reusable

---

## Installation

```bash
dotnet add package CoreSystem.Serialization
```

---

## Quick Start

```csharp
builder.Services.AddCoreSerialization();

public sealed class OrderService(IPayloadSerializer serializer)
{
    public byte[] Serialize(Order order) => serializer.Serialize(order);

    public Order? Deserialize(byte[] payload) => serializer.Deserialize<Order>(payload);
}
```

---

## Formats

| Serializer | Class | Notes |
|---|---|---|
| JSON | `JsonPayloadSerializer` | Default, human readable |
| MessagePack | `MessagePackPayloadSerializer` | Compact binary |
| Protobuf | `ProtobufPayloadSerializer` | Schema-driven binary |

---

## Contributing

Issues, discussions and pull requests are welcome.
