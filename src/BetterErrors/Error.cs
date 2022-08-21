namespace BetterErrors;

public record Error(string Message, string? Code = nameof(Error)) : IError;
