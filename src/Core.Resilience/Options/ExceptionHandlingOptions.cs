namespace Core.Resilience.Options;

public abstract class ExceptionHandlingOptions
{
    private readonly HashSet<Type> _handledExceptions = [];

    public IReadOnlyCollection<Type> HandledExceptions => _handledExceptions;

    protected void AddHandledException(Type exceptionType)
    {
        ArgumentNullException.ThrowIfNull(exceptionType);

        if (!typeof(Exception).IsAssignableFrom(exceptionType))
        {
            throw new ArgumentException(
                $"Type '{exceptionType.FullName}' must derive from Exception.",
                nameof(exceptionType));
        }

        _handledExceptions.Add(exceptionType);
    }
}