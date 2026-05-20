using Core.Idempotency.Models;

namespace Core.Idempotency.Storage;

public interface IIdempotencyStorage
{
    Task<IdempotencyResponse?> GetAsync(string key);
    Task SaveAsync(string key, IdempotencyResponse response, TimeSpan expiration);
}