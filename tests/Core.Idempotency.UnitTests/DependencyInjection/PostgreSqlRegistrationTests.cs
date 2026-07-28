using Core.Idempotency.Abstractions;
using Core.Idempotency.DependencyInjection;
using Core.Idempotency.Storage.PostgreSQL;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.UnitTests.DependencyInjection;

public class PostgreSqlRegistrationTests
{
    [Fact]
    public void AddCoreIdempotency_Should_Register_PostgreSqlProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreIdempotency(options =>
        {
            options.Enabled = true;
            options.Provider = IdempotencyProviderType.PostgreSQL;
            options.PostgreSql.ConnectionString =
                "Host=localhost;Database=test;Username=test;Password=test";
        });

        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetRequiredService<PostgresIdempotencyStorage>();
        provider.GetRequiredService<IIdempotencyStorage>();
    }

    [Fact]
    public void AddCoreIdempotency_Should_Throw_When_PostgreSqlProvider_Has_No_ConnectionString()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var action = () => services.AddCoreIdempotency(options =>
        {
            options.Enabled = true;
            options.Provider = IdempotencyProviderType.PostgreSQL;
        });

        // Assert
        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage(IdempotencyMessages.PostgreSqlConnectionStringRequired);
    }
}
