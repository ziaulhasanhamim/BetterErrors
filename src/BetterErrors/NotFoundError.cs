namespace BetterErrors;

public record NotFoundError(string Message, string? Code = nameof(NotFoundError)) 
    : Error(Message, Code);
