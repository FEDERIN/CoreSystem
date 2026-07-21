using Core.Resilience.Options;
using Polly;

namespace Core.Resilience.Internal.Extensions;

internal static class BackoffTypeExtensions
{
    public static DelayBackoffType ToPolly(
        this BackoffType backoffType)
    {
        return backoffType switch
        {
            BackoffType.Constant => DelayBackoffType.Constant,
            BackoffType.Linear => DelayBackoffType.Linear,
            BackoffType.Exponential => DelayBackoffType.Exponential,
            _ => throw new ArgumentOutOfRangeException(nameof(backoffType))
        };
    }
}