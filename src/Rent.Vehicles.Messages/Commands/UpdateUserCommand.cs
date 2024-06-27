using System.ComponentModel.DataAnnotations;

using MessagePack;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record UpdateUserCommand : Command
{
    [MessagePack.Key(1)]
    [Required]
    public Guid Id
    {
        get;
        init;
    }

    [MessagePack.Key(2)]
    [Base64String]
    public required string LicenseImage
    {
        get;
        init;
    }
}