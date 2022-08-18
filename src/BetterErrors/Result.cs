namespace BetterErrors;

public readonly struct Result<T>
{
    private readonly T? _obj;
    private readonly IError? _error;

    public Result(T obj)
    {
        Success = true;
        _error = default;
        _obj = obj;
    }

    public Result(IError err)
    {
        Success = false;
        _obj = default;
        _error = err;
    }

    public bool Success { get; }

    public T Value => Success switch
    {
        true => _obj!,
        false => throw new InvalidOperationException("Result doesn't contain success value")
    };

    public IError Error => Success switch
    {
        true => throw new InvalidOperationException("Result doesn't contain error value"),
        false => _error!
    };

    public TMap Match<TMap>(Func<T, TMap> success, Func<IError, TMap> failure) =>
        Success ? success(_obj!) : failure(_error!);

    public void Switch(Action<T> success, Action<IError> failure)
    {
        if (Success)
        {
            success(_obj!);
            return;
        }
        failure(_error!);
    }

    public Result<TMap> Map<TMap>(Func<T, Result<TMap>> success) => Success switch
    {
        true => success(_obj!),
        false => _error!.ToResult<TMap>()
    };

    public static implicit operator Result<T>(T obj) => new(obj);

    public static implicit operator Result<T>(FailureError err) => new(err);
}