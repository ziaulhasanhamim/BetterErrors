namespace BetterErrors;

public static class ErrorExtensions
{
    public static Result<T> ToResult<T>(this IError err) => new(err);
}