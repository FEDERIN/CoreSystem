namespace Core.Resilience.Options;

/// <summary>
/// Represents the base class for exception handling options in a resilience strategy.
/// </summary>
public abstract class ExceptionHandlingOptions
{
    private readonly HashSet<Type> _handledExceptions = [];

    /// <summary>
    /// Gets the collection of exception types that are handled by the resilience strategy.
    /// </summary>
    public IReadOnlyCollection<Type> HandledExceptions => _handledExceptions;

    /// <summary>
    /// Gets or sets a value indicating whether to include inner exceptions.
    /// </summary>
    public bool IncludeInnerExceptions { get; set; }

    /// <summary>
    /// Adds an exception type to the collection of handled exceptions.
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <exception cref="ArgumentException"></exception>
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