using Core.Idempotency.Models;

namespace Core.Idempotency.Abstractions;

internal interface IIdempotencyStorage
{
    Task<IdempotencyResponse?> GetAsync(
        string key,
        CancellationToken ct = default);

    Task SetAsync(
        string key,
        IdempotencyResponse response,
        TimeSpan? expiration = null,
        CancellationToken ct = default);
}