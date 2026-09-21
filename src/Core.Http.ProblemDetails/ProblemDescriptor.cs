namespace Core.Http.ProblemDetails;

/// <summary>Describes a domain-agnostic HTTP problem.</summary>
public sealed record ProblemDescriptor
{
    private int _status;
    private string _errorCode = null!;
    private string _title = null!;
    private string _detail = null!;

    /// <summary>Initializes a validated HTTP problem descriptor.</summary>
    public ProblemDescriptor(int status, string errorCode, string title, string detail, string? type = null)
    {
        Status = status;
        ErrorCode = errorCode;
        Title = title;
        Detail = detail;
        Type = type;
    }

    /// <summary>Gets the HTTP status code, constrained to the valid HTTP range.</summary>
    public int Status
    {
        get => _status;
        init => _status = value is >= 100 and <= 599
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), value, "The status code must be between 100 and 599.");
    }

    /// <summary>Gets the application error code.</summary>
    public string ErrorCode
    {
        get => _errorCode;
        init => _errorCode = ValidateRequired(value, nameof(ErrorCode));
    }

    /// <summary>Gets the human-readable problem title.</summary>
    public string Title
    {
        get => _title;
        init => _title = ValidateRequired(value, nameof(Title));
    }

    /// <summary>Gets the human-readable problem detail.</summary>
    public string Detail
    {
        get => _detail;
        init => _detail = ValidateRequired(value, nameof(Detail));
    }

    /// <summary>Gets the optional RFC 9457 problem type URI.</summary>
    public string? Type { get; init; }

    private static string ValidateRequired(string? value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value;
    }
}
