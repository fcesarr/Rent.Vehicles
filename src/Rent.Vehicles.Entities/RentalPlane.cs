using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Vehicles.Entities;

[Table("rentalPlanes", Schema = "vehicles")]
public class RentalPlane : Entity
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
