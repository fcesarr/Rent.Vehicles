using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record UpdateRentEvent : Messages.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }

    [Key(2)]
    public required DateTime EstimatedDate
    {
        get;
        set;
    }
}
