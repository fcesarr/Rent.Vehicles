namespace Rent.Vehicles.Consumers.Exceptions;

public class SpecificYearException : NoRetryException
{
    public SpecificYearException(string? message) : base(message)
    {
    }
}
