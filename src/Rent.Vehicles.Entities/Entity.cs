using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rent.Vehicles.Entities;

public class Entity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [BsonElement("Created")]
    public DateTime Created { get; } = DateTime.UtcNow;
}