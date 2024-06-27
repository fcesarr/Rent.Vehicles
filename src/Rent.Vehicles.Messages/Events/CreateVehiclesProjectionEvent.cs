using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record CreateVehiclesProjectionEvent : CreateVehiclesEvent;