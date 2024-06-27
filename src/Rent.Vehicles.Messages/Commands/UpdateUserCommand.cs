using System.ComponentModel.DataAnnotations;

using MessagePack;

using Rent.Vehicles.Messages.Types;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record UpdateUserCommand : Command
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
    public required string Name
    {
        get;
        set;
    }

    [MessagePack.Key(3)]
    [Required]
    public required string Number
    {
        get;
        set;
    }

    [MessagePack.Key(4)]
    [Required]
    public required DateTime Birthday
    {
        get;
        set;
    }

    [MessagePack.Key(5)]
    [Required]
    public required string LicenseNumber
    {
        get;
        set;
    }

    [MessagePack.Key(6)]
    [Required]
    public LicenseType LicenseType
    {
        get;
        set;
    }
}
