using MessagePack;

using Rent.Vehicles.Messages.Types;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record Event : Messages.Event
{
    [Key(1)]
    public required string Type { get; set; }

    [Key(2)]
    public required StatusType StatusType { get; set; }

    [Key(3)]
    public required string Message { get; set; }
}