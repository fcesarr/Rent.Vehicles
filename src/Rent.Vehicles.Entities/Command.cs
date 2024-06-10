using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities;

public class Command : Entity
{
    public Guid SagaId { get; } = Guid.NewGuid();
    public required ActionType Type { get; set; }
    public required IEnumerable<byte> Data { get; set; }
}