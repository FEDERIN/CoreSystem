namespace Core.DistributedCache.Storage.Abstractions;

internal interface ICacheTagIndex
{
    Task AddAsync(
        string key,
        IReadOnlyCollection<string> tags,
        CancellationToken cancellationToken = default);

    Task RemoveKeyAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task InvalidateTagAsync(
        string tag,
        Func<string, CancellationToken, Task> removeEntry,
        CancellationToken cancellationToken = default);
}