using Core.Idempotency.Abstractions;
using Core.Idempotency.Options;
using Core.Idempotency.Storage.PostgreSQL;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.DependencyInjection;

internal static class PostgreSqlRegistration
{
    public static IServiceCollection AddIdempotencyPostgreSql(
        this IServiceCollection services,
        IdempotencyOptions options)
    {
        if (string.IsNullOrWhiteSpace(
                options.PostgreSql.ConnectionString))
        {
            throw new InvalidOperationException(
                "PostgreSQL connection string is required.");
        }

        services.AddSingleton<PostgresIdempotencyStorage>();

        services.AddSingleton<IIdempotencyStorage>(
            sp => sp.GetRequiredService<PostgresIdempotencyStorage>());

        return services;
    }
}