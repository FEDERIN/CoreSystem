# 🧩 Extensibility

`CoreSystem.Cache.Rehydration` separates the source of recoverable entries from
the target used to store them.

## Rehydration Source

`IRehydrationSource` defines:

```csharp
IEnumerable<CacheRehydrationEntry> GetEntries();

Task RemoveForRehydrationAsync(
    string key,
    CancellationToken ct = default);
```

The current implementation is `MemoryRehydrationSource`.

## Rehydration Target

`IRehydrationTarget` defines:

```csharp
Task StoreAsync(
    CacheRehydrationEntry entry,
    CancellationToken ct = default);
```

The current implementation is `PrimaryRehydrationTarget`, which writes through
`ICacheStorageResolver.Primary`.

## Public Extensibility

The source and target abstractions are internal. The current code therefore
does not provide a public SDK for replacing the rehydration source or target.

The package's extension points are internal implementation boundaries rather
than a public provider extension model.
