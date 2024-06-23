using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record CreateVehiclesForSpecificYearEvent : CreateVehiclesEvent;

[MessagePackObject]
public record CreateVehiclesSuccessEvent : CreateVehiclesEvent;

[MessagePackObject]
public record DeleteVehiclesSuccessEvent : DeleteVehiclesEvent;

[MessagePackObject]
public record UpdateVehiclesSuccessEvent : UpdateVehiclesEvent;
