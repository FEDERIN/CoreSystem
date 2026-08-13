namespace Core.Cache.Abstractions;

internal interface IPrimaryHealthState
{
    bool IsHealthy { get; }
}