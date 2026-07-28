using Core.Idempotency.Abstractions;

namespace Core.Idempotency.DependencyInjection;

internal static class IdempotencyMessages
{
    public const string MissingRegistration =
        "Core.Idempotency has not been registered. Call services.AddCoreIdempotency(...) before app.UseCoreIdempotency().";

    public const string PostgreSqlConnectionStringRequired =
        "PostgreSQL connection string is required.";

    public static string UnsupportedProvider(IdempotencyProviderType provider) =>
    $"Provider '{provider}' is not supported.";

    public const string RedisConfigurationRequired =
    "Redis configuration is required when using the Redis idempotency provider.";
}