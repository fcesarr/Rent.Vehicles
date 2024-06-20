
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Messages.Commands;

[MessagePack.MessagePackObject]
public record CreateVehiclesCommand : Command
{
    [MessagePack.Key(1)]
    [JsonIgnore]
    public Guid Id { get; set; }
    
    [MessagePack.Key(2)]
    [Range(2020, 2025, ErrorMessage = "Year should be between 2020 and 2025")]
    public required int Year { get; init; }

    [MessagePack.Key(3)]
    [Required]
    public required string Model { get; init; }

    [MessagePack.Key(4)]
    [Required]
    public required string LicensePlate { get; init; }

    [MessagePack.Key(5)]
    [Required]
    [EnumDataType(typeof(VehicleType))]
    public required VehicleType Type { get; init; }
}