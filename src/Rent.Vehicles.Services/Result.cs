namespace Rent.Vehicles.Services;

public record Result<T>
{
    public readonly Exception? Exception;
    public readonly bool IsSuccess;
    public readonly T? Value;

    protected Result(bool isSuccess, T? value, Exception? exception)
    {
        IsSuccess = isSuccess;
        Value = value;
        Exception = exception;
    }

    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    public static implicit operator Result<T>(Exception exception)
    {
        return Failure(exception);
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }

    public static Result<T> Failure(Exception? exception)
    {
        return new Result<T>(false, default, exception!);
    }
}
