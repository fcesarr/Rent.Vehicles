using Rent.Vehicles.Services.Exceptions;

namespace Rent.Vehicles.Consumers.Exceptions;

public class SpecificYearException : NoRetryException
{
    public SpecificYearException(string? message) : base(message)
    {
    }

    public SpecificYearException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
