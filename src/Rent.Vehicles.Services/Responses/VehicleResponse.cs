namespace Rent.Vehicles.Services.Responses;

public record VehicleResponse
{
    public required Guid Id
    {
        get;
        init;
    }

    public required int Year
    {
        get;
        init;
    }

    public required string Model
    {
        get;
        init;
    }

    public required string LicensePlate
    {
        get;
        set;
    }

    public required string Type
    {
        get;
        init;
    }

    public required bool IsRented
    {
        get;
        set;
    } = false;
}