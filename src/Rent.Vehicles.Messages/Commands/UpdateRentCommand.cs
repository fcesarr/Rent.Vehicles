using System.ComponentModel.DataAnnotations;

using MessagePack;

using Rent.Vehicles.Lib;
using Rent.Vehicles.Lib.Attributes;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record UpdateRentCommand : Command
{
    [MessagePack.Key(1)]
    [Required]
    public Guid Id
    {
        get;
        init;
    }

    [MessagePack.Key(2)]
    [Required]
    [DateTimeMinorCurrentDate(ErrorMessage = "Invalid date")]
    public required DateTime EstimatedDate
    {
        get;
        set;
    }
}
