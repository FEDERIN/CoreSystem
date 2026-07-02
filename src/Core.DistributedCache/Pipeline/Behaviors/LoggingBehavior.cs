using Core.DistributedCache.Pipeline.Contexts;
using Core.DistributedCache.Pipeline.Delegates;
using Core.DistributedCache.Pipeline.Interfaces;
using Microsoft.Extensions.Logging;

namespace Core.DistributedCache.Pipeline.Behaviors;

public sealed class LoggingBehavior(
    ILogger<LoggingBehavior> logger)
    : ICacheBehavior
{
    public async Task InvokeAsync(CacheContext context, CacheDelegate next)
    {
        logger.LogDebug(
            "Executing cache operation for key {Key} on {Storage}",
            context.Key,
            context.Storage?.GetType().Name);

        try
        {
            await next(context);

            logger.LogDebug(
                "Cache operation completed for key {Key}",
                context.Key);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Cache operation failed for key {Key}",
                context.Key);

            throw;
        }
    }
}