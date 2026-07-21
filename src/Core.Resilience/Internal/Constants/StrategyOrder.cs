namespace Core.Resilience.Internal.Constants;

internal static class StrategyOrder
{
    public const int Timeout = 1;

    public const int Retry = 2;

    public const int CircuitBreaker = 3;
}