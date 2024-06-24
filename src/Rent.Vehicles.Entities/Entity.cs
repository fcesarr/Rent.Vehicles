using System.ComponentModel.DataAnnotations;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rent.Vehicles.Entities;

public class Entity
{
    private DateTime _created = DateTime.UtcNow;

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [BsonElement("Created")]
    [Required]
    public DateTime Created
	{
		get => _created;

		private set => _created = value == default ? DateTime.UtcNow : value;
	}
}