using Polly;
using System.Reflection;

namespace Core.Resilience.Internal.Helpers;

internal static class PredicateBuilderFactory
{
    private static readonly MethodInfo HandleMethod =
        typeof(PredicateBuilder)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(m =>
                m.Name == nameof(PredicateBuilder.Handle) &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 0);

    public static PredicateBuilder Create(
        IReadOnlyCollection<Type> exceptionTypes)
    {
        ArgumentNullException.ThrowIfNull(exceptionTypes);

        var predicate = new PredicateBuilder();

        foreach (var exceptionType in exceptionTypes)
        {
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
            {
                throw new InvalidOperationException(
                    $"Type '{exceptionType.FullName}' must derive from Exception.");
            }

            HandleMethod
                .MakeGenericMethod(exceptionType)
                .Invoke(predicate, null);
        }

        return predicate;
    }
}