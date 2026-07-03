using Core.Cache.Pipeline.Contexts;
using Core.Cache.Pipeline.Delegates;

namespace Core.Cache.Pipeline.Abstractions;

public interface ICachePipeline
{
    Task ExecuteAsync(
        CacheContext context,
        CacheDelegate terminal);
}