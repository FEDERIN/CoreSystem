using Core.Cache.Abstractions;
using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Contexts;

namespace Core.Cache.Services;

internal sealed class CoreCache(
    ICachePipeline pipeline,
    ICacheStorageResolver resolver)
    : ICoreCache
{
    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken ct = default)
    {
        var context = new GetCacheContext<T>
        {
            Key = key,
            Storage = resolver.Primary,
            CancellationToken = ct
        };

        await pipeline.ExecuteAsync(
            context,
            static ctx => ctx.ExecuteAsync());

        return context.Result;
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default)
    {
        return pipeline.ExecuteAsync(
            new SetCacheContext<T>
            {
                Key = key,
                Value = value,
                Expiration = expiration,
                Tags = tags,
                Storage = resolver.Primary,
                CancellationToken = ct
            },
            static ctx => ctx.ExecuteAsync());
    }

    public async Task<bool> ExistsAsync(
        string key,
        CancellationToken ct = default)
    {
        var context = new ExistsCacheContext
        {
            Key = key,
            Storage = resolver.Primary,
            CancellationToken = ct
        };

        await pipeline.ExecuteAsync(
            context,
            static ctx => ctx.ExecuteAsync());

        return context.Exists;
    }

    public Task RemoveAsync(
        string key,
        CancellationToken ct = default)
    {
        return pipeline.ExecuteAsync(
            new RemoveCacheContext
            {
                Key = key,
                Storage = resolver.Primary,
                CancellationToken = ct
            },
            static ctx => ctx.ExecuteAsync());
    }

    public Task InvalidateByTagAsync(
        string tag,
        CancellationToken ct = default)
    {
        return pipeline.ExecuteAsync(
            new InvalidateTagCacheContext
            {
                Key = $"tag:{tag}",
                Tag = tag,
                Storage = resolver.Primary,
                CancellationToken = ct
            },
            static ctx => ctx.ExecuteAsync());
    }

    public async Task<T?> GetOrAddAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default)
    {
        var context = new GetOrAddCacheContext<T>
        {
            Key = key,
            Factory = factory,
            Expiration = expiration,
            Tags = tags,
            Storage = resolver.Primary,
            CancellationToken = ct
        };

        await pipeline.ExecuteAsync(
            context,
            static ctx => ctx.ExecuteAsync());

        return context.Result;
    }
}