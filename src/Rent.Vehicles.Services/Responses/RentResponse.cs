namespace Rent.Vehicles.Services.Responses;

public record RentResponse
{
    public required Guid Id
    {
        get;
        init;
    }

    public required int NumberOfDays
    {
        get;
        init;
    }

    public required decimal DailyCost
    {
        get;
        init;
    }

    public required DateTime StartDate
    {
        get;
        init;
    }

    public required DateTime EndDate
    {
        get;
        init;
    }

    public required DateTime EstimatedDate
    {
        get;
        init;
    }

    public required decimal PreEndDatePercentageFine
    {
        get;
        init;
    }

    public required decimal PostEndDateFine
    {
        get;
        init;
    }

    public required decimal Cost
    {
        get;
        init;
    }

    public required VehicleResponse Vehicle
    {
        get;
        init;
    }

    public required UserResponse User
    {
        get;
        init;
    }
}
