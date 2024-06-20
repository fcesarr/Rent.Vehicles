using Microsoft.EntityFrameworkCore;

using Rent.Vehicles.Entities.Contexts.Interfaces;

namespace Rent.Vehicles.Entities.Contexts;

public class RentVehiclesContext : DbContext, IDbContext
{
	public RentVehiclesContext(DbContextOptions options)
		: base(options)
	{
		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
	}

    public virtual DbSet<Command>? CommandSet { get; set; }

	public virtual DbSet<Vehicle>? VehicleSet { get; set; }

    public virtual DbSet<User>? UserSet { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}