namespace Core.Cache.Rehydration.DependencyInjection;

internal static class RehydrationMessages
{
    public const string CacheRegistrationRequired =
        "Core.Cache registration is required before enabling rehydration.";

    public const string PrimaryRegistrationRequired =
        "An external cache provider is required before enabling rehydration.";
}