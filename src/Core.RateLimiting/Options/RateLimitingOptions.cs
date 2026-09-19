using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Core.RateLimiting.Options;

/// <summary>
/// Provides configuration options for rate-limiting policies.
/// </summary>
public sealed class RateLimitingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether rate limiting is enabled.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to enable rate limiting; otherwise, <see langword="false"/>.
    /// The default value is <see langword="true"/>.
    /// </value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the name of the rate-limiting policy.
    /// </summary>
    /// <value>
    /// The policy name used to identify the configured rate limiter.
    /// The default value is <c>core-fixed-window</c>.
    /// </value>
    public string PolicyName { get; set; } = "core-fixed-window";

    /// <summary>
    /// Gets or sets the maximum number of permits available within the configured window.
    /// </summary>
    /// <value>
    /// The maximum number of requests allowed during the configured window.
    /// The default value is <c>100</c>.
    /// </value>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Gets or sets the duration of the rate-limiting window.
    /// </summary>
    /// <value>
    /// The amount of time during which the configured permit limit applies.
    /// The default value is one minute.
    /// </value>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the maximum number of requests that can wait in the queue
    /// when all permits are currently in use.
    /// </summary>
    /// <value>
    /// The maximum number of queued requests. The default value is <c>0</c>,
    /// which means that requests are not queued.
    /// </value>
    public int QueueLimit { get; set; }

    /// <summary>
    /// Gets or sets the order in which queued requests are processed.
    /// </summary>
    /// <value>
    /// A <see cref="QueueProcessingOrder"/> value that determines how queued
    /// requests are selected for processing. The default value is
    /// <see cref="QueueProcessingOrder.OldestFirst"/>.
    /// </value>
    public QueueProcessingOrder QueueProcessingOrder { get; set; } =
        QueueProcessingOrder.OldestFirst;

    /// <summary>
    /// Gets or sets a value indicating whether permits are replenished automatically.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to automatically replenish permits; otherwise,
    /// <see langword="false"/>. The default value is <see langword="true"/>.
    /// </value>
    public bool AutoReplenishment { get; set; } = true;

    /// <summary>
    /// Gets or sets the claim type used to identify the subject to which
    /// rate limiting is applied.
    /// </summary>
    /// <value>
    /// The claim type used to resolve the rate-limiting subject.
    /// The default value is <see cref="ClaimTypes.NameIdentifier"/>.
    /// </value>
    public string SubjectClaimType { get; set; } = ClaimTypes.NameIdentifier;

    /// <summary>
    /// Copies every rate-limiting option from the specified source instance.
    /// </summary>
    /// <param name="source">
    /// The options instance to copy.
    /// </param>
    /// <returns>
    /// The current <see cref="RateLimitingOptions"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
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

    /// <summary>
    /// Validates the configured rate-limiting options.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <see cref="PolicyName"/> or <see cref="SubjectClaimType"/>
    /// is null, empty, or consists only of white-space characters.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <see cref="PermitLimit"/> is less than or equal to zero,
    /// <see cref="Window"/> is less than or equal to <see cref="TimeSpan.Zero"/>,
    /// or <see cref="QueueLimit"/> is negative.
    /// </exception>
    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(PolicyName))
            throw new ArgumentException(
                "Rate-limit policy name is required.",
                nameof(PolicyName));

        if (PermitLimit <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(PermitLimit),
                PermitLimit,
                "Rate-limit permit limit must be greater than zero.");

        if (Window <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(
                nameof(Window),
                Window,
                "Rate-limit window must be greater than zero.");

        if (QueueLimit < 0)
            throw new ArgumentOutOfRangeException(
                nameof(QueueLimit),
                QueueLimit,
                "Rate-limit queue limit cannot be negative.");

        if (string.IsNullOrWhiteSpace(SubjectClaimType))
            throw new ArgumentException(
                "Rate-limit subject claim type is required.",
                nameof(SubjectClaimType));
    }
}