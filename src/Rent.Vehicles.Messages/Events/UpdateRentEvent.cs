using MessagePack;

using Rent.Vehicles.Messages.Types;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record UpdateRentEvent : Messages.Event
{
    [Key(1)]
    public required Guid Id { get; init; }

    [Key(2)]
    public required DateTime EndDate { get; set; }
}