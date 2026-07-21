namespace Core.Resilience.Options;

public sealed class RetryOptions : ExceptionHandlingOptions
{
    public bool Enabled { get; set; } = true;

    public int MaxRetryAttempts { get; set; } = 3;

    public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(200);

    public BackoffType BackoffType { get; set; } = BackoffType.Exponential;

    public bool UseJitter { get; set; }

    public RetryOptions Handle<TException>()
        where TException : Exception
    {
        AddHandledException(typeof(TException));
        return this;
    }

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