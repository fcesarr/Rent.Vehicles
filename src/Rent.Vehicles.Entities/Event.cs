using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Rent.Vehicles.Entities.Types;

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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required StatusType StatusType { get; set; }

    [BsonElement("Message")]
    public required string Message { get; set; }
}