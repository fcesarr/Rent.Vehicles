using System.ComponentModel.DataAnnotations.Schema;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities;

[Table("users", Schema = "vehicles")]
public class User : Entity
{
    public required string Name
    {
        get;
        set;
    }

    public required string Number
    {
        get;
        set;
    }

    public required DateTime Birthday
    {
        get;
        set;
    }

    public required string LicenseNumber
    {
        get;
        set;
    }

    public LicenseType LicenseType
    {
        get;
        set;
    }

    public required string LicensePath
    {
        get;
        set;
    }
}
