namespace Core.Resilience.Options;

public sealed class CircuitBreakerOptions : ExceptionHandlingOptions
{
    public bool Enabled { get; set; } = true;

    public int MinimumThroughput { get; set; } = 10;

    public double FailureRatio { get; set; } = 0.5;

    public TimeSpan SamplingDuration { get; set; } = TimeSpan.FromSeconds(30);

    public TimeSpan BreakDuration { get; set; } = TimeSpan.FromSeconds(15);

    public CircuitBreakerOptions Handle<TException>()
        where TException : Exception
    {
        AddHandledException(typeof(TException));
        return this;
    }

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