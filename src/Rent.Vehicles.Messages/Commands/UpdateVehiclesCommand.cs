using System.ComponentModel.DataAnnotations;

namespace Rent.Vehicles.Messages.Commands;

[MessagePack.MessagePackObject]
public record UpdateVehiclesCommand : Command
{
    [MessagePack.Key(1)]
    [Required]
    public required Guid Id { get; init; }

    [MessagePack.Key(2)]
    [Required]
    public required string LicensePlate { get; init; }
}