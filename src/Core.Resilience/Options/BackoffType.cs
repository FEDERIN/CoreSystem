namespace Core.Resilience.Options;

/// <summary>Backoff strategies supported by retry policies.</summary>
public enum BackoffType
{
    /// <summary>Waits a fixed delay between attempts.</summary>
    Constant,

    /// <summary>Increases the delay linearly with each attempt.</summary>
    Linear,

    /// <summary>Increases the delay exponentially with each attempt.</summary>
    Exponential
}
