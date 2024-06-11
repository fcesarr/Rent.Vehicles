using MessagePack;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record DeleteVehiclesEvent : Event
{
    [Key(1)]
    public required Guid Id { get; init; }
}