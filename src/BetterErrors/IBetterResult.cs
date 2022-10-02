namespace BetterErrors;

public interface IBetterResult
{
    public bool IsSuccess { get; }

    public bool IsFailure { get; }

    public IError Error { get; }

    TMap Match<TMap>(Func<TMap> success, Func<IError, TMap> failure);

    void Switch(Action success, Action<IError> failure);
}
