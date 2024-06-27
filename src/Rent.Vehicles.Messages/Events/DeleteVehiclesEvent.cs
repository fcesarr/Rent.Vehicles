using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record DeleteVehiclesEvent : Messages.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }
}