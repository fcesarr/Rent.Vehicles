using MongoDB.Bson.Serialization.Attributes;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities;

public class Vehicle : Entity
{

    [BsonElement("Year")]
    public int Year { get; init; }
    
    [BsonElement("Model")]
    public string Model { get; init; }

    [BsonElement("LicensePlate")]
    public string LicensePlate { get; init; }

    [BsonElement("Type")]
    public VehicleType Type { get; init; }
}