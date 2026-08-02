using Polly.CircuitBreaker;
using Polly.Retry;

namespace Core.Resilience.Internal.Helpers;

internal static class ExceptionHandlingPredicateFactory
{
    public static Func<RetryPredicateArguments<object>, ValueTask<bool>>
        CreateRetryPredicate(IReadOnlyCollection<Type> exceptionTypes)
    {
        return args => ValueTask.FromResult(
            Matches(args.Outcome.Exception, exceptionTypes));
    }

    public static Func<CircuitBreakerPredicateArguments<object>, ValueTask<bool>>
        CreateCircuitBreakerPredicate(IReadOnlyCollection<Type> exceptionTypes)
    {
        return args => ValueTask.FromResult(
            Matches(args.Outcome.Exception, exceptionTypes));
    }

    private static bool Matches(
        Exception? exception,
        IReadOnlyCollection<Type> exceptionTypes)
    {
        return exception is not null &&
               ExceptionMatcher.Matches(exception, exceptionTypes);
    }
}
