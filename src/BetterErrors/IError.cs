namespace BetterErrors;

public interface IError
{
    public string? Code { get; }

    public string Message { get; }
}