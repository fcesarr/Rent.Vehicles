namespace Rent.Vehicles.Services.Exceptions;

public class NullException : NoRetryException
{
    public NullException(string? message) : base(message)
    {
    }

    public NullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}