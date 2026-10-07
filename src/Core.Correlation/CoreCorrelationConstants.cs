namespace Core.Correlation;

/// <summary>Constants used by CoreSystem correlation middleware.</summary>
public static class CoreCorrelationConstants
{
    /// <summary>The correlation identifier stored in the HttpContext for the current request.</summary>
    public const string HttpContextItemKey = "CoreSystem.CorrelationId";

    /// <summary>Name of the property used to enrich logs with the correlation identifier.</summary>
    public const string LogPropertyName = "correlationId";
}
