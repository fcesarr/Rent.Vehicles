using MongoDB.Bson.Serialization.Attributes;

namespace Rent.Vehicles.Entities;

public class Event : Entity
{
    [BsonElement("SagaId")]
    public required Guid SagaId { get; set; }

    [BsonElement("Name")]
    public required string Name { get; set; }

    [BsonElement("StatusType")]
    public required StatusType StatusType { get; set; }

    [BsonElement("Message")]
    public required string Message { get; set; }
}

public enum StatusType
{
    Success,
    Fail
}