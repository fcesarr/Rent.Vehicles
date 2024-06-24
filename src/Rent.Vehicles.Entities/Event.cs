using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Types;

namespace Rent.Vehicles.Entities;

[Table("events", Schema = "events")]
public class Event : Entity
{
    public required Guid SagaId { get; set; }
    public required string Name { get; set; }
    public required StatusType StatusType { get; set; }
    public required string Message { get; set; }
    public required SerializerType SerializerType { get; set; }
    [JsonIgnore]
    public required IList<byte> Data { get; set; }
}