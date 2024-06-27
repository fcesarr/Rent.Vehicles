using MessagePack;

namespace Rent.Vehicles.Messages.Projections.Events;

[MessagePackObject]
public record UpdateRentProjectionEvent : Event;
