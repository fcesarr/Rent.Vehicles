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

	public virtual DbSet<Vehicle>? VehicleSet { get; set; }

    public virtual DbSet<Command>? CommandSet { get; set; }

    public virtual DbSet<Event>? EventSet { get; set; }

    public virtual DbSet<VehiclesForSpecificYear>? VehiclesForSpecificYearSet { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the primary key for the Event entity
        modelBuilder.Entity<Event>();

        // Configure the relationship
        modelBuilder.Entity<Command>()
            .HasMany(c => c.Events)
            .WithOne(e => e.Command)
            .HasForeignKey(e => e.SagaId)
            .HasPrincipalKey(c => c.SagaId);
    }
}