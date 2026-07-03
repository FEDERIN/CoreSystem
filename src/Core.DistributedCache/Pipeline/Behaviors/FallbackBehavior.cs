using Core.Cache.Abstractions;
using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Contexts;
using Core.Cache.Pipeline.Delegates;
using Microsoft.Extensions.Logging;

namespace Core.Cache.Pipeline.Behaviors;

internal sealed class FallbackBehavior(
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