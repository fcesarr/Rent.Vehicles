using MongoDB.Bson.Serialization.Attributes;

namespace Rent.Vehicles.Entities.Projections;


public class RentProjection : Entity
{
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

    public required DateTime StartDate
    {
        get;
        set;
    }

    public required DateTime EndDate
    {
        get;
        set;
    }

    public required DateTime EstimatedDate
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

    public required decimal Cost
    {
        get;
        set;
    }

    public required VehicleProjection Vehicle
    {
        get;
        set;
    }

    public required UserProjection User
    {
        get;
        set;
    }
}
