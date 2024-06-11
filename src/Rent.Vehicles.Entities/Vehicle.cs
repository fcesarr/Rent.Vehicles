using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities;

public class Vehicle : Entity
{
    public required int Year { get; init; }

    public required string Model { get; init; }

    public required string LicensePlate { get; init; }

    public required VehicleType Type { get; init; }
}