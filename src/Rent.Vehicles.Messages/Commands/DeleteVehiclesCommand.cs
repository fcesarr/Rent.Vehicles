using MessagePack;

namespace Rent.Vehicles.Messages.Commands;

[MessagePackObject]
public record DeleteVehiclesCommand : Message
{
    public DeleteVehiclesCommand()
    {
        SagaId = Guid.NewGuid();
    }

    [Key(1)]
    public required Guid Id { get; init; }
}