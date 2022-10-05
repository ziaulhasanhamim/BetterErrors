namespace BetterErrors.Internals;

public interface IBetterResult<T> : IBetterObjectResult
{
    TMap Match<TMap>(Func<T, TMap> success, Func<IError, TMap> failure);

    void Switch(Action<T> success, Action<IError> failure);

    Result<TMap> Map<TMap>(Func<T, Result<TMap>> success);

    ValueTask<Result<TMap>> Map<TMap>(Func<T, ValueTask<Result<TMap>>> success);
}