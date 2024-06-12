using MongoDB.Bson.Serialization.Attributes;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities;

public class Vehicle : Entity
{

    [BsonElement("Year")]
    public required int Year { get; init; }
    
    [BsonElement("Model")]
    public required string Model { get; init; }

    [BsonElement("LicensePlate")]
    public required string LicensePlate { get; init; }

    [BsonElement("Type")]
    public required VehicleType Type { get; init; }
}