using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record UpdateVehiclesEvent : Messages.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }

    [Key(2)]
    public required string LicensePlate
    {
        get;
        init;
    }
}
