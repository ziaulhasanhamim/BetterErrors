using BetterErrors.Internals;

namespace BetterErrors;

/// <summary>
/// A discriminated union of T or IError
/// </summary>
public readonly struct Result<T> : IBetterResult<T>
{
    private readonly T? _value;
    private readonly IError? _error;

    internal Result(T value)
    {
        IsSuccess = true;
        _error = default;
        _value = value;
    }

    internal Result(IError err)
    {
        IsSuccess = false;
        _value = default;
        _error = err;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess switch
    {
        true => _value!,
        false => throw new InvalidOperationException("Result doesn't contain success value")
    };

    public IError Error => IsSuccess switch
    {
        true => throw new InvalidOperationException("Result doesn't contain error value"),
        false => _error!
    };

    bool IBetterResult.IsSuccess => throw new NotImplementedException();

    bool IBetterResult.IsFailure => throw new NotImplementedException();

    IError IBetterResult.Error => throw new NotImplementedException();

    public TMap Match<TMap>(Func<T, TMap> success, Func<IError, TMap> failure) =>
        IsSuccess ? success(_value!) : failure(_error!);

    public void Switch(Action<T> success, Action<IError> failure)
    {
        if (IsSuccess)
        {
            success(_value!);
            return;
        }
        failure(_error!);
    }

    public Result<TMap> Map<TMap>(Func<T, Result<TMap>> success) => IsSuccess switch
    {
        true => success(_value!),
        false => _error!.ToResult<TMap>()
    };

    public ValueTask<Result<TMap>> Map<TMap>(Func<T, ValueTask<Result<TMap>>> success) => IsSuccess switch
    {
        true => success(_value!),
        false => ValueTask.FromResult(_error!.ToResult<TMap>())
    };

    TMap IBetterResult.Match<TMap>(Func<TMap> success, Func<IError, TMap> failure) =>
        Match(_ => success(), failure);

    void IBetterResult.Switch(Action success, Action<IError> failure) =>
        Switch(_ => success(), failure);

    object IBetterObjectResult.GetResultValue() => Value!;

    public static implicit operator Result<T>(T value) => Result.From(value);

    public static implicit operator Result<T>(Error err) => Result.FromErr<T>(err);

    public static implicit operator Result<T>(AggregateError err) => Result.FromErr<T>(err);

    public static implicit operator Result<T>(IError[] errors) => new AggregateError(errors);

    public static implicit operator Result<T>(List<IError> errors) => new AggregateError(errors);
}
