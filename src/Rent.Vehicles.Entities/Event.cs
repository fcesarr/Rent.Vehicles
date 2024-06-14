using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson.Serialization.Attributes;

namespace Rent.Vehicles.Entities;

[Table("events", Schema = "events")]
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

    [BsonElement("Command")]
    public virtual Command Command { get; set; }
}

public enum StatusType
{
    Success,
    Fail
}