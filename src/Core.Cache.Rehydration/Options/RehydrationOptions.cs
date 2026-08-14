namespace Core.Cache.Rehydration.Options;

/// <summary>
/// Represents the configuration options for cache rehydration.
/// </summary>
public sealed class RehydrationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether cache rehydration is enabled.
    /// </summary>
    /// <remarks>
    /// When enabled, cache rehydration will attempt to restore entries that were temporarily
    /// stored in the fallback provider after the primary provider becomes available again.
    /// </remarks>
    public bool Enabled { get; set; } = true;

    ///// <summary>
    ///// Gets or sets the interval between cache rehydration cycles.
    ///// </summary>
    ///// <remarks>
    ///// Cache rehydration attempts to restore entries that were temporarily
    ///// stored in the fallback provider after the primary provider becomes
    ///// available again.
    ///// </remarks>
    public TimeSpan Interval { get; set; } =
        TimeSpan.FromSeconds(30); 

    public void CopyFrom(RehydrationOptions other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Enabled = other.Enabled;
        Interval = other.Interval;
    }
}