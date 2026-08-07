using Microsoft.Extensions.Configuration;


namespace CoreSystem.Samples.Infrastructure.Configuration;

internal static class RedisConfigurationFactory
{
    //public static Action<ConfigurationOptions> Create(
    //    IConfiguration configuration,
    //    string connectionName)
    //{
    //    var section = configuration.GetSection($"RedisConnections:{connectionName}");

    //    Validate(section, connectionName);

    //    return options =>
    //    {
    //        options.EndPoints.Add(section["Host"]!);
    //        options.Password = section["Password"];
    //    };
    //}

    private static void Validate(
        IConfigurationSection section,
        string connectionName)
    {
        if (!section.Exists())
        {
            throw new InvalidOperationException(
                $"The configuration section 'RedisConnections:{connectionName}' was not found.");
        }

        if (string.IsNullOrWhiteSpace(section["Host"]))
        {
            throw new InvalidOperationException(
                $"RedisConnections:{connectionName}:Host is required.");
        }

        if (string.IsNullOrWhiteSpace(section["Password"]))
        {
            throw new InvalidOperationException(
                $"RedisConnections:{connectionName}:Password is required.");
        }
    }
}