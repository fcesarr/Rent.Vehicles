using System.ComponentModel.DataAnnotations;

using MessagePack;

using Rent.Vehicles.Lib;
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
    public string? Name
    {
        get;
        set;
    }

    [MessagePack.Key(3)]
    public string? Number
    {
        get;
        set;
    }

    [MessagePack.Key(4)]
    public DateTime? Birthday
    {
        get;
        set;
    }

    [MessagePack.Key(5)]
    public string? LicenseNumber
    {
        get;
        set;
    }

    [MessagePack.Key(6)]
    public LicenseType? LicenseType
    {
        get;
        set;
    }
}
