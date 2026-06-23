using Core.Idempotency.Models;

namespace Core.Idempotency.Abstractions;

public interface IIdempotencyStorage
{
    Task<IdempotencyResponse?> GetAsync(string key);
    Task SaveAsync(string key, IdempotencyResponse response, TimeSpan expiration);
}