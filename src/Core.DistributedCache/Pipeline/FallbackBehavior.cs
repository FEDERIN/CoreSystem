using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Contexts;
using Microsoft.Extensions.Logging;

namespace Core.DistributedCache.Pipeline;

public sealed class FallbackBehavior(
    ICacheStorageResolver resolver,
    ILogger<FallbackBehavior> logger) : ICacheBehavior
{
    private readonly ICacheStorageResolver _resolver = resolver;
    private readonly ILogger<FallbackBehavior> _logger = logger;

    public async Task InvokeAsync(CacheContext context, CacheDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Primary storage failed. Switching to fallback.");

            context.Exception = ex;
            context.Storage = _resolver.Fallback;

            await next(context);
        }
    }
}