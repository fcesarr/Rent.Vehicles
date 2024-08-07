using MongoDB.Bson.Serialization.Attributes;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities.Projections;

public class UserProjection : Entity
{
    [BsonElement("Name")]
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
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
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
    public required LicenseType LicenseType
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
