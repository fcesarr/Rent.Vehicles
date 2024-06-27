using System.ComponentModel.DataAnnotations;

using MessagePack;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record UpdateUserLicenseImageCommand : Command
{
    [MessagePack.Key(1)]
    [Required]
    public required Guid Id
    {
        get;
        init;
    }

    [MessagePack.Key(2)]
    [Required]
    [Base64String]
    public required string LicenseImage
    {
        get;
        init;
    }
}
