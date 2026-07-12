namespace Core.Cache.DependencyInjection;

internal static class CacheMessages
{
    public const string MissingRegistration =
        "Core.Cache has not been registered. Call services.AddCoreCache(...) before app.UseCoreCache().";
}