using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MessagePack;

using Rent.Vehicles.Lib;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record CreateRentCommand : Command
{
    [MessagePack.Key(1)]
    [JsonIgnore]
    public Guid Id
    {
        get;
        init;
    } = Guid.NewGuid();

    [MessagePack.Key(2)]
    [Required]
    public required Guid UserId
    {
        get;
        set;
    }

    [MessagePack.Key(3)]
    [Required]
    public required Guid RentPlaneId
    {
        get;
        set;
    }
}
