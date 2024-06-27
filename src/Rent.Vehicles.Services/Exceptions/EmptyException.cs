namespace Rent.Vehicles.Services.Exceptions;

public class EmptyException : NoRetryException
{
    public EmptyException(string? message) : base(message)
    {
    }

    public EmptyException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}