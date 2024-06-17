using MessagePack;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record Event : Messages.Event
{
    [Key(1)]
    public required string Name { get; set; }

    [Key(2)]
    public required StatusType StatusType { get; set; }

    [Key(3)]
    public required string Message { get; set; }
}