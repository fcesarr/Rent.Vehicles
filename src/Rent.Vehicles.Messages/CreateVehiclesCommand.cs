using MessagePack;

namespace Rent.Vehicles.Messages;

[MessagePackObject]
public record CreateVehiclesCommand : Message
{
    [Key(1)]
    public required DateTime Year { get; init; } 
}