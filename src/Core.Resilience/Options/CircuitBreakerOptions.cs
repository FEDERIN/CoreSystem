namespace Core.Resilience.Options;

/// <summary>
/// Represents the options for configuring a circuit breaker strategy in a resilience pipeline.
/// </summary>
public sealed class CircuitBreakerOptions : ExceptionHandlingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the circuit breaker strategy is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum number of requests that must be made before the circuit breaker can calculate the failure ratio.
    /// </summary>
    public int MinimumThroughput { get; set; } = 10;

    /// <summary>
    /// Gets or sets the failure ratio threshold that will trigger the circuit breaker to open.
    /// </summary>
    public double FailureRatio { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the duration of the sampling period for the circuit breaker.
    /// </summary>
    public TimeSpan SamplingDuration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the duration of the break period for the circuit breaker.
    /// </summary>
    public TimeSpan BreakDuration { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Configures the circuit breaker to handle exceptions of the specified type.
    /// </summary>
    /// <typeparam name="TException">The type of the exception to handle.</typeparam>
    /// <returns>The current instance of the circuit breaker options.</returns>
    public CircuitBreakerOptions Handle<TException>()
        where TException : Exception
    {
        AddHandledException(typeof(TException));
        return this;
    }

    /// <summary>
    /// Configures the circuit breaker to handle exceptions of the specified types.
    /// </summary>
    /// <param name="exceptionTypes"></param>
    /// <returns></returns>
    public CircuitBreakerOptions Handle(params Type[] exceptionTypes)
    {
        ArgumentNullException.ThrowIfNull(exceptionTypes);

        foreach (var type in exceptionTypes)
        {
            AddHandledException(type);
        }

        return this;
    }
}