namespace Core.Resilience.Options;

/// <summary>
/// Represents the options for configuring a timeout strategy in a resilience pipeline.
/// </summary>
public sealed class TimeoutOptions
{

    private TimeSpan _timeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// <see langword="public"/> Gets or sets the timeout duration for the timeout strategy.
    /// </summary>
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