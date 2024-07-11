namespace Rent.Vehicles.Services.Responses;

public record RentalPlaneResponse
{
    public required Guid Id
    {
        get;
        init;
    }

    public required int NumberOfDays
    {
        get;
        set;
    }

    public required decimal DailyCost
    {
        get;
        set;
    }

    public required decimal PreEndDatePercentageFine
    {
        get;
        set;
    }

    public required decimal PostEndDateFine
    {
        get;
        set;
    }
}
