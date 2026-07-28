using Core.Idempotency.Models;

namespace Core.Idempotency.Abstractions;

internal interface IIdempotencyStorage
{
    Task<IdempotencyEntry?> GetAsync(
        string key,
        CancellationToken ct = default);

    Task SetAsync(
        string key,
        IdempotencyEntry entry,
        TimeSpan? expiration = null,
        CancellationToken ct = default);
}