namespace BetterErrors.Internals;

public interface IBetterObjectResult : IBetterResult
{
    object GetResultValue();
}