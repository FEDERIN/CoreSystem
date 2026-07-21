namespace Core.Resilience.Options;

public sealed class TimeoutOptions
{
    private TimeSpan _timeout = TimeSpan.FromSeconds(30);

    public TimeSpan Timeout
    {
        get => _timeout;
        set
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Timeout must be greater than zero.");
            }

            _timeout = value;
        }
    }
}