namespace Core.Cache.Redis.Diagnostics;

internal enum HealthTransition
{
    None,
    BecameHealthy,
    BecameUnhealthy
}