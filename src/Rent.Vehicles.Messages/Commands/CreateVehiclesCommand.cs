
using System.Text.Json.Serialization;

using MessagePack;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record CreateVehiclesCommand : Command
{
    [Key(1)]
    [JsonIgnore]
    public Guid Id { get; set; }

    [Key(2)]
    [System.ComponentModel.DataAnnotations.Range(2020, 2025, ErrorMessage = "Year should be between 2020 and 2025")]
    public required int Year { get; init; }

    [Key(3)]
    public required string Model { get; init; }

    [Key(4)]
    public required string LicensePlate { get; init; }

    [Key(5)]
    public required VehicleType Type { get; init; }
}