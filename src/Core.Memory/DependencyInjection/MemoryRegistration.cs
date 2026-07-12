using Core.Memory.Synchronization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Memory.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the Core.Memory services
/// into the dependency injection container.
/// </summary>
/// <remarks>
/// This extension registers the default in-memory synchronization
/// components required by <c>CoreSystem.Memory</c>.
///
/// The library provides lightweight keyed asynchronous locks that
/// coordinate concurrent operations within a single process.
///
/// Registered services:
/// <list type="bullet">
/// <item>
/// <description>
/// <see cref="IAsyncKeyLock"/> as a singleton.
/// </description>
/// </item>
/// </list>
///
/// Typical usage:
///
/// <code>
/// builder.Services.AddCoreMemory();
/// </code>
///
/// After registration, inject <see cref="IAsyncKeyLock"/>
/// wherever keyed synchronization is required.
///
/// <code>
/// public sealed class OrderService(IAsyncKeyLock lockProvider)
/// {
/// }
/// </code>
/// </remarks>
public static class MemoryRegistration
{
    /// <summary>
    /// Registers the Core.Memory services required for in-process
    /// keyed asynchronous synchronization.
    /// </summary>
    /// <param name="services">
    /// The application's dependency injection container.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance so that
    /// additional service registrations can be chained.
    /// </returns>
    /// <remarks>
    /// This method registers the default implementation of
    /// <see cref="IAsyncKeyLock"/> as a singleton.
    ///
    /// The synchronization provided by this library is limited to the
    /// current process. It should not be used to coordinate work across
    /// multiple application instances or servers.
    /// </remarks>
    public static IServiceCollection AddCoreMemory(
        this IServiceCollection services)
    {
        // Synchronization
        services.AddSingleton<ILockRegistry, MemoryLockRegistry>();

        services.AddSingleton<IAsyncKeyLock, MemoryLockProvider>();

        return services;
    }
}


