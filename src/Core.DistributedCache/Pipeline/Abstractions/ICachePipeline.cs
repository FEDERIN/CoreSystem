using Core.DistributedCache.Pipeline.Contexts;
using Core.DistributedCache.Pipeline.Delegates;

namespace Core.DistributedCache.Pipeline.Abstractions;

public interface ICachePipeline
{
    Task ExecuteAsync(
        CacheContext context,
        CacheDelegate terminal);
}