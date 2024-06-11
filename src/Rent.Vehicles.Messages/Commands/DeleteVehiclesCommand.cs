using MessagePack;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record DeleteVehiclesCommand : Command
{
    [Key(1)]
    public required Guid Id { get; init; }
}