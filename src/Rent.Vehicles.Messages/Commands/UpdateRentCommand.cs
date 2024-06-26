using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rent.Vehicles.Messages.Commands;

[MessagePack.MessagePackObject]
public record UpdateRentCommand : Command
{
    [MessagePack.Key(1)]
    [Required]
    public Guid Id { get; init; }

    [MessagePack.Key(2)]
    [Required]
    public required DateTime Data { get; set; }
}