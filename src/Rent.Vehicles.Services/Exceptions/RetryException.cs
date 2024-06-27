namespace Rent.Vehicles.Services.Exceptions;

public class RetryException : Exception
{
    public RetryException(string? message) : base(message)
    {
    }

    public RetryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}