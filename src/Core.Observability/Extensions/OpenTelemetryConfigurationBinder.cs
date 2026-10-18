using Core.Observability.Options;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Core.Observability.Extensions;

/// <summary>
/// Binds a single OpenTelemetry configuration section without letting a malformed value take the host down.
/// </summary>
internal static class OpenTelemetryConfigurationBinder
{
    /// <summary>
    /// Binds a child of the <see cref="OpenTelemetryOptions.SectionName"/> section onto a new instance of
    /// <typeparamref name="T"/>, reporting invalid values instead of throwing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each consumer binds only its own subsection (<c>OpenTelemetry:Tracing</c>, <c>OpenTelemetry:Metrics</c>,
    /// <c>OpenTelemetry:Logging</c>) rather than the whole <c>OpenTelemetry</c> section, so an invalid value in
    /// one exporter cannot break the others.
    /// </para>
    /// <para>
    /// Configuration binding throws <see cref="InvalidOperationException"/> when a value cannot be converted
    /// to the target property type. Observability must never be the reason an application fails to boot, so
    /// an invalid value is reported and the type defaults are used instead.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The options type to bind onto.</typeparam>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="childName">The child section name, for example <c>Tracing</c>.</param>
    /// <param name="options">The bound options, or a default-initialised instance when the section is
    /// missing or invalid.</param>
    /// <returns><c>true</c> when the section was bound successfully; <c>false</c> when it was missing or
    /// invalid, in which case <paramref name="options"/> holds the type defaults.</returns>
    public static bool TryBindChild<T>(IConfiguration configuration, string childName, out T options)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(configuration);

        options = new T();
        string path = $"{OpenTelemetryOptions.SectionName}:{childName}";
        IConfigurationSection section = configuration.GetSection(path);

        // A missing section is not an error: it simply means the feature stays at its defaults.
        if (!section.Exists())
        {
            return false;
        }

        try
        {
            section.Bind(options);
            return true;
        }
        catch (InvalidOperationException exception)
        {
            Trace.WriteLine(
                $"[Observability] The '{path}' configuration section contains an invalid value and could not " +
                $"be bound ({exception.Message}). Using defaults instead.");

            options = new T();
            return false;
        }
    }
}