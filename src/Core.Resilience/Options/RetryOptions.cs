namespace Core.Resilience.Options;

/// <summary>
/// Represents the options for configuring a retry strategy in a resilience pipeline.
/// </summary>
public sealed class RetryOptions : ExceptionHandlingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the retry strategy is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the delay between retry attempts.
    /// </summary>
    public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(200);
    
    /// <summary>
    /// Gets or sets the type of backoff strategy to use.
    /// </summary>
    public BackoffType BackoffType { get; set; } = BackoffType.Exponential;

    /// <summary>
    /// Gets or sets a value indicating whether to use jitter in the retry strategy.
    /// </summary>
    public bool UseJitter { get; set; }

    /// <summary>
    /// Configures the retry strategy to handle exceptions of the specified type.
    /// </summary>
    /// <typeparam name="TException">The type of exception to handle.</typeparam>
    /// <returns>The current instance of the retry options.</returns>
    public RetryOptions Handle<TException>()
        where TException : Exception
    {
        AddHandledException(typeof(TException));
        return this;
    }

    /// <summary>
    /// Configures the retry strategy to handle exceptions of the specified types.
    /// </summary>
    /// <param name="exceptionTypes"></param>
    /// <returns></returns>
    public RetryOptions Handle(params Type[] exceptionTypes)
    {
        ArgumentNullException.ThrowIfNull(exceptionTypes);

        foreach (var type in exceptionTypes)
        {
            AddHandledException(type);
        }

        return this;
    }
}