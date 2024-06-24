namespace Rent.Vehicles.Consumers.Exceptions;

public class NoRetryException : Exception
{
    public NoRetryException(string? message) : base(message)
    {
    }

    public NoRetryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
