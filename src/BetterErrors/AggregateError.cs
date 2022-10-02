namespace BetterErrors;

public sealed record AggregateError(IEnumerable<IError> Errors) : IError
{
    public string Message => throw new InvalidOperationException($"AggregateError represents a collection of errors. It doesn't contain any message");

    public string Code => nameof(AggregateError);
}