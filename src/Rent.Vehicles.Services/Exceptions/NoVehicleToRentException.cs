namespace Rent.Vehicles.Services.Exceptions;

public class NoVehicleToRentException : NoRetryException
{
    public NoVehicleToRentException(string? message) : base(message)
    {
    }
}
