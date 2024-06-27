using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Rent.Vehicles.Entities.Contexts.Interfaces;

namespace Rent.Vehicles.Entities.Contexts;

public class RentVehiclesContext : DbContext, IDbContext, IUnitOfWorkerContext
{
    public RentVehiclesContext(DbContextOptions options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public virtual DbSet<Command>? CommandSet
    {
        get;
        set;
    }

    public virtual DbSet<Vehicle>? VehicleSet
    {
        get;
        set;
    }

    public virtual DbSet<User>? UserSet
    {
        get;
        set;
    }

    public virtual DbSet<Event>? EventSet
    {
        get;
        set;
    }

    public virtual DbSet<RentalPlane>? RentalPlaneSet
    {
        get;
        set;
    }

    public virtual DbSet<Rent>? RentSet
    {
        get;
        set;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        IDbContextTransaction transaction = await Database.BeginTransactionAsync(cancellationToken);
        await Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.RollbackTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.CommitTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar a entidade Event
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasOne<Command>()
                .WithMany()
                .HasForeignKey(e => e.SagaId)
                .HasPrincipalKey(e => e.SagaId)
                .OnDelete(DeleteBehavior.Cascade); // ou o comportamento de deleção que você preferir
        });

        modelBuilder.Entity<RentalPlane>().HasData(
            new RentalPlane
            {
                NumberOfDays = 7, DailyCost = 30.0m, PreEndDatePercentageFine = 1.20m, PostEndDateFine = 50.0m
            },
            new RentalPlane
            {
                NumberOfDays = 15, DailyCost = 28.0m, PreEndDatePercentageFine = 1.40m, PostEndDateFine = 50.0m
            },
            new RentalPlane
            {
                NumberOfDays = 30, DailyCost = 22.0m, PreEndDatePercentageFine = 1.0m, PostEndDateFine = 50.0m
            },
            new RentalPlane
            {
                NumberOfDays = 45, DailyCost = 20.0m, PreEndDatePercentageFine = 1.0m, PostEndDateFine = 50.0m
            }
            ,
            new RentalPlane
            {
                NumberOfDays = 50, DailyCost = 18.0m, PreEndDatePercentageFine = 1.0m, PostEndDateFine = 50.0m
            }
        );

        modelBuilder.Entity<Rent>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rent>()
            .HasOne(r => r.Vehicle)
            .WithMany()
            .HasForeignKey(r => r.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}