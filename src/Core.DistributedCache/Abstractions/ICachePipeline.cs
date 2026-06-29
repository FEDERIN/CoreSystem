namespace Core.DistributedCache.Abstractions;

public interface ICachePipeline
{
    Task ExecuteAsync(
        CacheContext context,
        CacheDelegate terminal);
}