namespace Core.Idempotency.Exceptions;

/// <summary>
/// The incoming request does not match the fingerprint
/// associated with the existing idempotency entry.
/// </summary>
public sealed class IdempotencyFingerprintMismatchException
    : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyFingerprintMismatchException"/> class.
    /// </summary>
    public IdempotencyFingerprintMismatchException()
        : base("The request fingerprint does not match the existing idempotency entry.")
    {
    }
}