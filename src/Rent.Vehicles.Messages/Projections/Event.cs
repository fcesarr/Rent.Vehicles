using MessagePack;

namespace Rent.Vehicles.Messages.Projections;

[MessagePackObject]
public record Event : Lib.Event
{
    [Key(1)]
    public required Guid Id
    {
        get;
        init;
    }
}
