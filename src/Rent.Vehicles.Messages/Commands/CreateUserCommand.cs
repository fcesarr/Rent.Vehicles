using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using MessagePack;

using Rent.Vehicles.Lib;
using Rent.Vehicles.Messages.Types;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record CreateUserCommand : Command
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
    public required string Name
    {
        get;
        init;
    }

    [MessagePack.Key(3)]
    [Required]
    public required string Number
    {
        get;
        init;
    }

    [MessagePack.Key(4)]
    [Required]
    public required DateTime Birthday
    {
        get;
        init;
    }

    [MessagePack.Key(5)]
    [Required]
    public required string LicenseNumber
    {
        get;
        init;
    }

    [MessagePack.Key(6)]
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required LicenseType LicenseType
    {
        get;
        init;
    }

    [MessagePack.Key(7)]
    [Base64String]
    public required string LicenseImage
    {
        get;
        init;
    }
}
