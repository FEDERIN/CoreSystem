using Microsoft.Extensions.Configuration;

namespace CoreSystem.Samples.Infrastructure.Configuration;

internal static class PostgreSqlConfigurationFactory
{
    public static string Create(
        IConfiguration configuration,
        string connectionName)
    {
        var section =
            configuration.GetSection(
                $"PostgreSqlConnections:{connectionName}");

        Validate(section, connectionName);

        return section["ConnectionString"]!;
    }

    private static void Validate(
        IConfigurationSection section,
        string connectionName)
    {
        if (!section.Exists())
        {
            throw new InvalidOperationException(
                $"The configuration section 'PostgreSqlConnections:{connectionName}' was not found.");
        }

        if (string.IsNullOrWhiteSpace(section["ConnectionString"]))
        {
            throw new InvalidOperationException(
                $"PostgreSqlConnections:{connectionName}:ConnectionString is required.");
        }
    }
}