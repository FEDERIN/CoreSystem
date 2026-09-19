using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Core.RateLimiting.Options;

public sealed class RateLimitingOptions
{
    public bool Enabled { get; set; } = true;
    public string PolicyName { get; set; } = "core-fixed-window";
    public int PermitLimit { get; set; } = 100;
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
    public int QueueLimit { get; set; }
    public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;
    public bool AutoReplenishment { get; set; } = true;
    public string SubjectClaimType { get; set; } = ClaimTypes.NameIdentifier;

    /// <summary>
    /// Copies every rate-limiting option from the specified source instance.
    /// </summary>
    /// <param name="source">The options instance to copy.</param>
    /// <returns>The current options instance.</returns>
    public RateLimitingOptions CopyFrom(RateLimitingOptions source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Enabled = source.Enabled;
        PolicyName = source.PolicyName;
        PermitLimit = source.PermitLimit;
        Window = source.Window;
        QueueLimit = source.QueueLimit;
        QueueProcessingOrder = source.QueueProcessingOrder;
        AutoReplenishment = source.AutoReplenishment;
        SubjectClaimType = source.SubjectClaimType;

        return this;
    }

    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(PolicyName))
            throw new ArgumentException("Rate-limit policy name is required.", nameof(PolicyName));
        if (PermitLimit <= 0)
            throw new ArgumentOutOfRangeException(nameof(PermitLimit), PermitLimit, "Rate-limit permit limit must be greater than zero.");
        if (Window <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(Window), Window, "Rate-limit window must be greater than zero.");
        if (QueueLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(QueueLimit), QueueLimit, "Rate-limit queue limit cannot be negative.");
        if (string.IsNullOrWhiteSpace(SubjectClaimType))
            throw new ArgumentException("Rate-limit subject claim type is required.", nameof(SubjectClaimType));
    }
}
