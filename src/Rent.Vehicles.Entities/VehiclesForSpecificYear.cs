using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Vehicles.Entities;

[Table("vehiclesForSpecificYear", Schema = "vehicles")]
public class VehiclesForSpecificYear : Vehicle;