namespace BetterErrors;

public readonly record struct FieldErrorInfo(
    string Key, 
    string Message, 
    string? ErrorCode = null
);

public record ValidationError(
    string Message, 
    IEnumerable<FieldErrorInfo> ErrorInfos, 
    string? Code = nameof(ValidationError)
) : FailureError(Message, Code);