using MessagePack;

namespace Rent.Vehicles.Lib;

[MessagePackObject]
public record Command : Message;
