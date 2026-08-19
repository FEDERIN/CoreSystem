# ⚙️ Configuration

This guide describes the configuration options exposed by
`CoreSystem.Cache.Rehydration`.

## Rehydration Options

The provider exposes `RehydrationOptions`:

```csharp
public bool Enabled { get; set; } = true;

public TimeSpan Interval { get; set; } =
    TimeSpan.FromSeconds(30);
```

Configure the component through `AddCoreCacheRehydration()`:

```csharp
services.AddCoreCacheRehydration(options =>
{
    options.Enabled = true;
    options.Interval = TimeSpan.FromSeconds(30);
});
```

### Enabled

Controls whether the rehydration services and hosted background service are
registered.

When disabled, the `RehydrationOptions` instance is still registered, but the
source, target, rehydrator, service, and hosted service are not registered.

### Interval

Defines the delay between background rehydration cycles.

The default value is:

```text
30 seconds
```

## Core Cache Requirement

Rehydration requires the `CacheOptions` instance registered by
`AddCoreCache()`.

If the core cache is disabled, rehydration registration returns without
registering the rehydration services.

## Primary Provider Requirement

An external cache provider implementing `IExternalCacheStorage` must already be
registered.

The rehydration target writes to `ICacheStorageResolver.Primary`, so the
rehydration package does not contain provider-specific configuration.
