namespace Core.Resilience.Internal.Helpers;

internal static class ExceptionMatcher
{
    public static bool Matches(
        Exception exception,
        IReadOnlyCollection<Type> exceptionTypes)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(exceptionTypes);

        return MatchesCore(exception, exceptionTypes);
    }

    private static bool MatchesCore(
        Exception? current,
        IReadOnlyCollection<Type> exceptionTypes)
    {
        while (current is not null)
        {
            foreach (var exceptionType in exceptionTypes)
            {
                if (exceptionType.IsInstanceOfType(current))
                {
                    return true;
                }
            }

            if (current is AggregateException aggregate)
            {
                foreach (var inner in aggregate.Flatten().InnerExceptions)
                {
                    if (MatchesCore(inner, exceptionTypes))
                    {
                        return true;
                    }
                }
            }

            current = current.InnerException;
        }

        return false;
    }
}