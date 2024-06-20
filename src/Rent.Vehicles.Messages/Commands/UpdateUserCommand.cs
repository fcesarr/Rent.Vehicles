using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rent.Vehicles.Messages.Commands;

[MessagePack.MessagePackObject]
public record UpdateUserCommand : Command
{
    [MessagePack.Key(1)]
    [Required]
    public Guid Id { get; init; }

    [MessagePack.Key(2)]
    [Base64String]
    public required string LicenseImage { get; init; }
}