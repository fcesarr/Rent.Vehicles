using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record CreateVehiclesForSpecificYearProjectionEvent : CreateVehiclesEvent;