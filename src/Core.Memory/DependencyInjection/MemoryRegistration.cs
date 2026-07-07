using Core.Memory.Synchronization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Memory.DependencyInjection;

public static class MemoryRegistration
{
    public static IServiceCollection AddCoreMemory(
        this IServiceCollection services)
    {
        // Synchronization
        services.AddSingleton<IMemoryLockProvider, MemoryLockProvider>();

        return services;
    }
}


