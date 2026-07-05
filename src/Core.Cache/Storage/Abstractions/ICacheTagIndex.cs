namespace Core.Cache.Storage.Abstractions;

internal interface ICacheTagIndex<TProvider>
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

    Task<IReadOnlyCollection<string>> GetKeysAsync(
        string tag,
        CancellationToken cancellationToken = default);

    Task<long> CountAsync(
        string tag,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string tag,
        CancellationToken cancellationToken = default);
}