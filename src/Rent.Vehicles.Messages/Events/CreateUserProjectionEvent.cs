using MessagePack;

namespace Rent.Vehicles.Messages.Events;

[MessagePackObject]
public record CreateUserProjectionEvent : CreateUserEvent;
