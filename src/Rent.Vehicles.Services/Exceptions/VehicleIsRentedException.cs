namespace Rent.Vehicles.Services.Exceptions;

public class VehicleIsRentedException : NoRetryException
{
    public VehicleIsRentedException(string? message) : base(message)
    {
    }
}
