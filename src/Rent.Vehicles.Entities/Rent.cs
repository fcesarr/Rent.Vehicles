using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Vehicles.Entities;

[Table("rents", Schema = "vehicles")]
public class Rent : Entity
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

    public required Guid VehicleId
    {
        get;
        set;
    }

    public Vehicle? Vehicle
    {
        get;
        set;
    }

    public required Guid UserId
    {
        get;
        set;
    }

    public User? User
    {
        get;
        set;
    }

    public bool IsActive
    {
        get;
        set;
    } = false;
}
