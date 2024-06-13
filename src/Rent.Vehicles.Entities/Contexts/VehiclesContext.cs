using Microsoft.EntityFrameworkCore;

using Rent.Vehicles.Entities.Contexts.Interfaces;

namespace Rent.Vehicles.Entities.Contexts;

public class VehiclesContext : DbContext, IDbContext
{
	public VehiclesContext(DbContextOptions<VehiclesContext> options)
		: base(options)
	{
		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
	}

	public virtual DbSet<Vehicle>? VehicleSet { get; set; }

    public virtual DbSet<Command>? CommandSet { get; set; }

    public virtual DbSet<VehiclesForSpecificYear>? VehiclesForSpecificYearSet { get; set; }
}