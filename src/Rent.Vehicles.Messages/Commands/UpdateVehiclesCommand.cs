using MessagePack;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record UpdateVehiclesCommand : Command
{
    [Key(1)]
    public required Guid Id { get; init; }

    [Key(2)]
    public required string LicensePlate { get; init; }
}