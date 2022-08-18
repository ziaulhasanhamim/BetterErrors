namespace BetterErrors;

public record FailureError(string Message, string? Code = nameof(FailureError)) : IError;