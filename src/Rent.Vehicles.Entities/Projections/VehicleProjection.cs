using MongoDB.Bson.Serialization.Attributes;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities.Projections;

public class VehicleProjection : Entity
{
    [BsonElement("Year")]
    public int Year
    {
        get;
        init;
    }

    [BsonElement("Model")]
    public string Model
    {
        get;
        init;
    }

    [BsonElement("LicensePlate")]
    public string LicensePlate
    {
        get;
        set;
    }

    [BsonElement("Type")]
    public VehicleType Type
    {
        get;
        init;
    }

    [BsonElement("IsRented")]
    public bool IsRented
    {
        get;
        set;
    } = false;
}
