# ⚙️ Configuration

This guide describes the configuration options exposed by
`CoreSystem.Cache.Redis`.

## Redis Options

The provider exposes `RedisOptions` with one configuration property:

```csharp
public Action<ConfigurationOptions>? Configuration { get; set; }
```

Configure the underlying StackExchange.Redis `ConfigurationOptions` through the
delegate:

```csharp
services.AddCoreCacheRedis(options =>
{
    options.Configuration = redis =>
    {
        redis.EndPoints.Add("localhost", 6379);
    };
});
```

The provider requires `Configuration` to be assigned. Registration fails when
the configuration delegate does not assign it.

## Core Cache Options

Redis also uses the `CacheOptions` registered by `AddCoreCache()`.

The `InstanceName` value is used as a Redis key prefix. For example:

```csharp
options.InstanceName = "orders";
```

produces Redis keys such as:

```text
orders:customer:1
```

If no instance name is configured, no prefix is added.

## Resilience

`CoreSystem.Resilience` defines a dedicated `PipelineType.Redis`.

When a Redis resilience pipeline is configured, `CoreSystem.Cache.Redis` adds
the following exceptions to its Retry and Circuit Breaker handling:

- `RedisConnectionException`
- `RedisTimeoutException`
- `TimeoutException`

The resilience strategies and their options belong to
`CoreSystem.Resilience`, not to `RedisOptions`.

## Rehydration

`CoreSystem.Cache.Rehydration` is configured separately through
`RehydrationOptions`.

When enabled with an external provider, entries temporarily stored in the
memory fallback can be restored to the primary provider.

The rehydration package owns its recovery configuration; these settings are not
part of `RedisOptions`.
