using MessagePack;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record CreateVehiclesCommand : Message
{
    public CreateVehiclesCommand()
    {
        SagaId = Guid.NewGuid();
    }

    [Key(1)]
    public required Guid Id { get; init; } = Guid.NewGuid();

    [Key(2)]
    public required int Year { get; init; }

    [Key(3)]
    public required string Model { get; init; }

    [Key(4)]
    public required string LicensePlate { get; init; }

    [Key(5)]
    public required VehicleType Type { get; init; }
}