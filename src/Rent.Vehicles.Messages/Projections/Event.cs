using MessagePack;

namespace Rent.Vehicles.Messages.Projections;

[MessagePackObject]
public record Event : Messages.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }
}
