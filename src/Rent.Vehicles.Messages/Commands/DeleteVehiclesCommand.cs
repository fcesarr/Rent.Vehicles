using System.ComponentModel.DataAnnotations;

using MessagePack;

using Rent.Vehicles.Lib;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record DeleteVehiclesCommand : Command
{
    [MessagePack.Key(1)]
    [Required]
    public required Guid Id
    {
        get;
        init;
    }
}
