using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record DeleteVehiclesEvent : Lib.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }
}
