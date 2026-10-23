# CoreSystem.Serialization

> **Production-ready serialization abstraction for .NET 8, .NET 9 and .NET 10.**

CoreSystem.Serialization provides a unified serialization abstraction for
.NET applications, allowing you to work with **JSON**, **MessagePack**, or
**Protocol Buffers** through a single API.

![NuGet](https://img.shields.io/nuget/v/CoreSystem.Serialization?style=for-the-badge)
![Downloads](https://img.shields.io/nuget/dt/CoreSystem.Serialization?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-blue?style=for-the-badge)

---

## Features

- Unified serialization abstraction (`IPayloadSerializer`)
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

| Serializer | Selected via `SerializerType` | Notes |
|---|---|---|
| JSON | `SerializerType.Json` | Default, human readable |
| MessagePack | `SerializerType.MessagePack` | Compact binary |
| Protobuf | `SerializerType.Protobuf` | Schema-driven binary |

The concrete serializer classes are internal; select one with
`options.DefaultSerializer` and consume it through `IPayloadSerializer`.

---

## Contributing

Issues, discussions and pull requests are welcome.
