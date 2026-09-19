namespace Core.Http.ProblemDetails;

/// <summary>Maps an application exception to an HTTP problem without coupling CoreSystem to domain types.</summary>
public interface IExceptionProblemMapper
{
    /// <summary>Maps the supplied exception to a problem response descriptor.</summary>
    ProblemDescriptor Map(Exception exception);
}
