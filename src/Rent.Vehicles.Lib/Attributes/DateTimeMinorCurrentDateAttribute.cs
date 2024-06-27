using System.ComponentModel.DataAnnotations;

namespace Rent.Vehicles.Lib.Attributes;

public class DateTimeMinorCurrentDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var d = Convert.ToDateTime(value);
        return d.Date >= DateTime.Now.Date;
    }
}
