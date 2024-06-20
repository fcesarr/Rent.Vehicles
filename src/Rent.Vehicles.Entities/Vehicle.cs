using System.ComponentModel.DataAnnotations.Schema;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities;

[Table("vehicles", Schema = "vehicles")]
public class Vehicle : Entity
{

    public int Year { get; init; }
    
    public string Model { get; init; }

    public string LicensePlate { get; set; }

    public VehicleType Type { get; init; }
}