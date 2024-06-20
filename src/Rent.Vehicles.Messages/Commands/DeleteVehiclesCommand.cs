
using System.ComponentModel.DataAnnotations;

namespace Rent.Vehicles.Messages.Commands;

[MessagePack.MessagePackObject]
public record DeleteVehiclesCommand : Command
{
    [MessagePack.Key(1)]
    [Required]
    public required Guid Id { get; init; }
}