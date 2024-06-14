using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rent.Vehicles.Entities;

[Table("events", Schema = "events")]
public class Event : Entity
{
    [BsonElement("SagaId")]
    [BsonRepresentation(BsonType.String)]
    public required Guid SagaId { get; set; }

    [BsonElement("Name")]
    public required string Name { get; set; }

    [BsonElement("StatusType")]
    [BsonRepresentation(BsonType.String)]
    public required StatusType StatusType { get; set; }

    [BsonElement("Message")]
    public required string Message { get; set; }

    [BsonIgnore]
    public virtual Command Command { get; set; }
}

public enum StatusType
{
    Success,
    Fail
}