using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Services.Responses;

public record UserResponse
{
    public required Guid Id
    {
        get;
        init;
    }

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

    public required LicenseType LicenseType
    {
        get;
        set;
    }

    public required string LicensePath
    {
        get;
        set;
    }

    public required DateTime Created
    {
        get;
        init;
    }
}
