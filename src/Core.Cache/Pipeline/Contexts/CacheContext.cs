using Core.Cache.Abstractions;
using Core.Cache.Storage;

namespace Core.Cache.Pipeline.Contexts;

public abstract class CacheContext
{
    public required string Key { get; init; }

    internal ICacheStorage Storage { get; set; } = default!;

    public CancellationToken CancellationToken { get; init; }

    public Exception? Exception { get; set; }
    
    public CacheEntryOptions EntryOptions { get; set; } = CacheEntryOptions.Default;

    public abstract Task ExecuteAsync();
}
