namespace Core.Cache.Pipeline.Behaviors;

internal enum CacheBehaviorOrder
{
    Logging = 100,
    Metrics = 200,
    Fallback = 300,
    Resilience = 400
}