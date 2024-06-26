
using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class RentDataService : DataService<Entities.Rent>, IRentDataService
{
    public RentDataService(ILogger<RentDataService> logger,
        IRentValidator validator,
        IRepository<Entities.Rent> repository) : base(logger, validator, repository)
    {
    }

    public async Task<Result<Entities.Rent>> CreateAsync(RentalPlane rentalPlane, Guid userId, Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.Now.Date.AddDays(1);

        var entity = new Entities.Rent
        {
            NumberOfDays = rentalPlane.NumberOfDays,
            DailyCost = rentalPlane.DailyCost,
            VehicleId = vehicleId,
            UserId = userId,
            StartDate = startDate,
            PreEndDatePercentageFine = rentalPlane.PreEndDatePercentageFine,
            PostEndDateFine = rentalPlane.PostEndDateFine,
            EstimatedDate = startDate.AddDays(rentalPlane.NumberOfDays),
            EndDate = startDate.AddDays(rentalPlane.NumberOfDays),
            Cost = rentalPlane.DailyCost * rentalPlane.NumberOfDays
        };

        return await CreateAsync(entity, cancellationToken);
    }

    public async Task<Result<Entities.Rent>> EstimateCostAsync(Guid id, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        var entityToUpdate = Update(entity.Value!, endDate);
        
        var result = await _validator.ValidateAsync(entityToUpdate, cancellationToken);

        if(!result.IsValid)
            return result.Exception;

        return entityToUpdate;
    }

    public async Task<Result<Entities.Rent>> UpdateAsync(Guid id, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

         if(!entity.IsSuccess)
            return entity.Exception!;
        
        var entityToUpdate = Update(entity.Value!, endDate);

        return await UpdateAsync(entityToUpdate, cancellationToken);
    }

    private Entities.Rent Update(Entities.Rent entity, DateTime endDate)
    {
        if(endDate.Date.Ticks < entity.EstimatedDate.Date.Ticks)
        {
            var diff = entity.EstimatedDate.Date - endDate.Date;

            var numberOfDays = (entity.NumberOfDays - diff.Days);

            var cost = entity.DailyCost * numberOfDays;

            entity.Cost = cost + ((diff.Days * entity.DailyCost) * entity.PreEndDatePercentageFine);
        }
        else if(endDate.Date.Ticks > entity.EstimatedDate.Date.Ticks)
        {
            var diff = endDate.Date - entity.EstimatedDate.Date;

            var cost = entity.PostEndDateFine * diff.Days;

            entity.Cost += cost;
        }

        entity.EstimatedDate = endDate.Date;

        return entity;
    }
}