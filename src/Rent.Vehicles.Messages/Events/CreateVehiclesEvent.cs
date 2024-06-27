using MessagePack;

using Rent.Vehicles.Messages.Types;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record CreateVehiclesEvent : Messages.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }

    [Key(2)]
    public required int Year
    {
        get;
        init;
    }

    [Key(3)]
    public required string Model
    {
        get;
        init;
    }

    [Key(4)]
    public required string LicensePlate
    {
        get;
        init;
    }

    [Key(5)]
    public required VehicleType Type
    {
        get;
        init;
    }
}
