# ⚙️ Configuration

This guide describes every configuration option available in
**FGutierrez.Core.DistributedCache**.

You'll learn how to configure:

- Cache providers
- Serialization
- Default expiration
- Redis connectivity
- Memory-only mode
- HTTP cache settings
- Cache rehydration
- appsettings.json integration

---

# Configuration Overview

The framework is configured through the `AddCoreDistributedCache()` extension.

```csharp
builder.Services.AddCoreDistributedCache(options =>
{
    // Configure the framework here
});
```

---

# Configuration Options

| Option | Description | Default |
|----------|-------------|---------|
| DefaultProvider | Default cache provider | Redis |
| InstanceName | Cache key prefix | null |
| DefaultExpiration | Default cache lifetime | 30 minutes |
| SerializerType | Serialization format | Json |
| MaxCacheableSize | Maximum cached HTTP response | 1 MB |
| RehydrationInterval | Redis recovery interval | 30 seconds |

---

# Choosing a Cache Provider

The framework supports multiple cache providers.

## Redis

Recommended for production workloads.

```csharp
builder.Services.AddCoreDistributedCache(options =>
{
    options.Redis.Configuration = redis =>
    {
        redis.EndPoints.Add("localhost:6379");
    };
});
```

### When to use Redis

- Distributed applications
- Multiple application instances
- Kubernetes
- Cloud deployments

---

## Memory

Ideal for:

- Local development
- Unit tests
- Small applications

```csharp
builder.Services.AddCoreDistributedCache(options =>
{
    options.Redis.Enabled = false;
});
```

---

# Default Provider

Choose which provider the framework should use.

```csharp
options.DefaultProvider =
    CacheProviderType.Redis;
```

or

```csharp
options.DefaultProvider =
    CacheProviderType.Memory;
```

---

# Instance Name

Prefixes every cache key.

```csharp
options.InstanceName = "CatalogApi";
```

Example generated key:

```text
CatalogApi:products:15
```

Using an instance name is recommended whenever multiple applications share the same Redis server.

---

# Cache Expiration

Configure the default cache lifetime.

```csharp
options.DefaultExpiration =
    TimeSpan.FromMinutes(30);
```

Individual cache operations can override this value.

---

# Serialization

Choose the serializer that best fits your application.

## JSON

```csharp
options.SerializerType =
    SerializerType.Json;
```

Recommended for:

- Simplicity
- Debugging
- Compatibility

---

## MessagePack

```csharp
options.SerializerType =
    SerializerType.MessagePack;
```

Recommended for:

- High-performance APIs
- Reduced payload size

---

## Protocol Buffers

```csharp
options.SerializerType =
    SerializerType.Protobuf;
```

Recommended for:

- Cross-platform communication
- Compact binary serialization

---

# Redis Configuration

The framework uses the native
`StackExchange.Redis.ConfigurationOptions`.

Example:

```csharp
options.Redis.Configuration = redis =>
{
    redis.EndPoints.Add("localhost:6379");

    redis.Password = "...";

    redis.Ssl = true;

    redis.ConnectTimeout = 5000;

    redis.AbortOnConnectFail = false;
};
```

---

# HTTP Cache

Maximum response size stored by the middleware.

```csharp
options.MaxCacheableSize =
    1024 * 1024;
```

Default:

```text
1 MB
```

---

# Cache Rehydration

Configure how frequently the framework checks whether Redis has recovered.

```csharp
options.RehydrationInterval =
    TimeSpan.FromSeconds(30);
```

---

# Using appsettings.json

```json
{
  "DistributedCache": {
    "DefaultProvider": "Redis",

    "InstanceName": "CatalogApi",

    "DefaultExpiration": "00:30:00",

    "SerializerType": "MessagePack",

    "Redis": {
      "Enabled": true,
      "Host": "localhost:6379",
      "Password": ""
    }
  }
}
```

Bind the configuration:

```csharp
var section =
    builder.Configuration.GetSection("DistributedCache");

builder.Services.AddCoreDistributedCache(options =>
{
    section.Bind(options);

    options.Redis.Configuration = redis =>
    {
        redis.EndPoints.Add(section["Redis:Host"]!);

        redis.Password =
            section["Redis:Password"];
    };
});
```

---

# Recommended Configurations

## Development

| Setting | Value |
|----------|-------|
| Provider | Memory |
| Serializer | Json |
| Expiration | 5 minutes |

---

## Staging

| Setting | Value |
|----------|-------|
| Provider | Redis |
| Serializer | Json |
| Expiration | 15 minutes |

---

## Production

| Setting | Value |
|----------|-------|
| Provider | Redis |
| Serializer | MessagePack |
| Expiration | 30 minutes |

---

# Best Practices

✅ Use Redis for distributed applications.

✅ Set an `InstanceName` when sharing Redis.

✅ Prefer MessagePack for performance-sensitive workloads.

✅ Use JSON during development if payload readability is important.

✅ Configure sensible expiration values.
