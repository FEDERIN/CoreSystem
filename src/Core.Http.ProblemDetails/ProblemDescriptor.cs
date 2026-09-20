namespace Core.Http.ProblemDetails;

/// <summary>Describes a domain-agnostic HTTP problem.</summary>
public sealed record ProblemDescriptor(
    int Status,
    string ErrorCode,
    string Title,
    string Detail,
    string? Type = null);
