using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Types;

namespace Rent.Vehicles.Entities;

public class Command : Entity
{
    public required Guid SagaId { get; set; }
    public required ActionType ActionType { get; set; }
    public required SerializerType SerializerType { get; set; }
    public required EntityType EntityType { get; set; }
    public required IEnumerable<byte> Data { get; set; }
}