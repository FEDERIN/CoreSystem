# ⚙️ Configuration

This guide describes the configuration options available in
**CoreSystem.Cache**.

You'll learn how to configure:

- Cache provider behavior
- Serialization
- Default expiration
- Cache key instance name
- HTTP cache settings
- Cache entry rehydration options

---

# Configuration Overview

The framework is configured through the `AddCoreCache()` extension.

```csharp
builder.Services.AddCoreCache(options =>
{
    // Configure the framework here
});
```

---

# Configuration Options

| Option | Description | Default |
|----------|-------------|---------|
| Enabled | Enables or disables the cache implementation | `true` |
| InstanceName | Optional prefix for cache keys | `null` |
| DefaultExpiration | Default cache lifetime | 30 minutes |
| SerializerType | Serialization format | JSON |
| MaxCacheableSize | Maximum cache entry size | 1 MB |

---

# Enable or Disable the Cache

The cache can be disabled while keeping the same `ICoreCache` abstraction available.

```csharp
builder.Services.AddCoreCache(options =>
{
    options.Enabled = false;
});
```

When disabled, `NoOpCoreCache` is registered.

`GetOrAddAsync()` continues to execute the factory, while cache read, write, remove, and invalidation operations become no-ops.

---

# Instance Name

Prefixes cache keys with an application or environment identifier.

```csharp
options.InstanceName = "CatalogApi";
```

The option is intended to help avoid key collisions when multiple applications share the same cache infrastructure.

---

# Cache Expiration

Configure the default cache lifetime.

```csharp
options.DefaultExpiration =
    TimeSpan.FromMinutes(30);
```

Individual cache operations can override this value.

```csharp
await cache.SetAsync(
    "products",
    products,
    TimeSpan.FromMinutes(5));
```

---

# Serialization

Choose the serializer used by the cache.

Serialization is provided by **CoreSystem.Serialization**.

## JSON

```csharp
options.SerializerType =
    SerializerType.Json;
```

JSON is the default serializer.

---

## MessagePack

```csharp
options.SerializerType =
    SerializerType.MessagePack;
```

---

## Protocol Buffers

```csharp
options.SerializerType =
    SerializerType.Protobuf;
```

---

# HTTP Cache

Configure the maximum allowed cache entry size.

```csharp
options.MaxCacheableSize =
    1024 * 1024;
```

Default:

```text
1 MB
```

The option is part of the HTTP/cache configuration, while the current `HttpCacheHandler` uses the configured `CacheOptions` for expiration and response caching behavior.

---

# Cache Entry Rehydration

`CacheEntryOptions` provides the `TrackForRehydration` flag.

The fallback pipeline uses:

```csharp
CacheEntryOptions.Rehydrate
```

when an operation is redirected from the primary storage to the fallback storage.

This marks the entry for rehydration when the external provider becomes available again.

The rehydration process itself belongs to the external-provider/recovery components and is not configured directly through the current `CacheOptions` class.

---

# External Providers

`CoreSystem.Cache` can operate with its Memory provider without an external cache provider.

When an external provider is registered, it becomes the primary storage and Memory is used as the fallback storage.

The external provider configuration is handled by the corresponding provider package and is not part of the `CacheOptions` class shown in the current `CoreSystem.Cache` implementation.

---

# Recommended Configurations

## Development

```csharp
builder.Services.AddCoreCache(options =>
{
    options.SerializerType = SerializerType.Json;
    options.DefaultExpiration = TimeSpan.FromMinutes(5);
});
```

---

## Production

The core package does not define a production provider configuration inside `CacheOptions`.

When using an external provider, configure that provider through its corresponding package and keep the common cache settings in `AddCoreCache()`.

---

# Best Practices

- Use an `InstanceName` when multiple applications share the same cache infrastructure.
- Configure a sensible default expiration.
- Override expiration for entries with different lifetimes.
- Choose the serializer according to the application's requirements.
- Use the Memory provider when an external distributed provider is not required.
- Configure external provider options in the corresponding provider package.
