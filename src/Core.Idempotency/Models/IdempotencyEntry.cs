namespace Core.Idempotency.Models;

public sealed class IdempotencyEntry
{
    public string? Fingerprint { get; init; }

    public required IdempotencyResponse Response { get; init; }
}