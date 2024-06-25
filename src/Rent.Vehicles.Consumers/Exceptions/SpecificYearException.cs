namespace Rent.Vehicles.Consumers.Exceptions;

public class SpecificYearException : RetryException
{
    public SpecificYearException(string? message) : base(message)
    {
    }
}
