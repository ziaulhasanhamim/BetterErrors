using BetterErrors.Internals;

namespace BetterErrors;

public readonly struct Result : IBetterResult
{
    private readonly Result<bool> _result;

    private Result(bool _)
    {
        _result = new(true);
    }

    internal Result(IError err)
    {
        _result = new(err);
    }

    public bool IsSuccess => _result.IsSuccess;

    public bool IsFailure => _result.IsFailure;

    public IError Error => _result.Error;

    public TMap Match<TMap>(Func<TMap> success, Func<IError, TMap> failure) =>
        _result.Match(_ => success(), failure);

    public void Switch(Action success, Action<IError> failure) =>
        _result.Switch(_ => success(), failure);

    public static implicit operator Result(Error err) => Result.FromErr(err);

    public static implicit operator Result(AggregateError err) => Result.FromErr(err);

    public static implicit operator Result(IError[] errors) => new AggregateError(errors);

    public static implicit operator Result(List<IError> errors) => new AggregateError(errors);

    public static Result FromErr(IError err) => new(err);

    public static readonly Result Success = new(true);

    public static Result<T> FromErr<T>(IError err) => new(err);

    public static Result<T> From<T>(T value) => new(value);
}