using MongoDB.Bson.Serialization.Attributes;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities.Projections;

public class UserProjection : Entity
{
    [BsonElement("Year")]
    public required string Name
    {
        get;
        set;
    }

    [BsonElement("Number")]
    public required string Number
    {
        get;
        set;
    }

    [BsonElement("Birthday")]
    public required DateTime Birthday
    {
        get;
        set;
    }

    [BsonElement("LicenseNumber")]
    public required string LicenseNumber
    {
        get;
        set;
    }

    [BsonElement("LicenseType")]
    public LicenseType LicenseType
    {
        get;
        set;
    }

    [BsonElement("LicensePath")]
    public required string LicensePath
    {
        get;
        set;
    }
}