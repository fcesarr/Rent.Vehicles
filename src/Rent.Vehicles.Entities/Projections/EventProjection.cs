using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Types;

namespace Rent.Vehicles.Entities.Projections;

public class EventProjection : Entity
{
    [BsonRepresentation(BsonType.String)]
    public required Guid SagaId
    {
        get;
        set;
    }

    public required string Name
    {
        get;
        set;
    }

    public required StatusType StatusType
    {
        get;
        set;
    }

    public required string Message
    {
        get;
        set;
    }

    public required SerializerType SerializerType
    {
        get;
        set;
    }
}
