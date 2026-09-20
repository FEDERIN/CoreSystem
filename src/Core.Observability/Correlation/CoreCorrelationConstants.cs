namespace Core.Observability.Correlation;

/// <summary>Constants used by CoreSystem correlation middleware.</summary>
public static class CoreCorrelationConstants
{
    public const string HttpContextItemKey = "CoreSystem.CorrelationId";
    public const string LogPropertyName = "correlationId";
}
