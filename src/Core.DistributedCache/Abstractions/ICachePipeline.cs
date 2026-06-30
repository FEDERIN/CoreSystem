using Core.DistributedCache.Contexts;

namespace Core.DistributedCache.Abstractions;

public interface ICachePipeline
{
    Task ExecuteAsync(
        CacheContext context,
        CacheDelegate terminal);
}