using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record CreateRentEvent : Messages.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }

    [Key(2)]
    public required Guid UserId
    {
        get;
        set;
    }

    [Key(3)]
    public required Guid RentPlaneId
    {
        get;
        set;
    }
}