using MessagePack;

namespace Rent.Vehicles.Messages;

[MessagePackObject]
public record Command : Message;