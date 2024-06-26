
using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.DataServices;

public class RentServiceDataService : DataService<Entities.Rent>, IRentDataService
{
    public RentServiceDataService(ILogger<RentServiceDataService> logger,
        IValidator<Entities.Rent> validator,
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

    public async Task<Result<Entities.Rent>> UpdateAsync(Guid id, DateTime date, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

         if(!entity.IsSuccess)
            return entity.Exception!;
        
        if(date.Ticks <= entity.Value.EstimatedDate.Date.Ticks)
        {
            var diff = entity.Value.EstimatedDate - date.Date;

            var numberOfDays = (entity.Value.NumberOfDays - diff.Days);

            var cost = entity.Value.DailyCost * numberOfDays;

            entity.Value.Cost = cost + ((diff.Days * entity.Value.DailyCost) * entity.Value.PreEndDatePercentageFine);
        }
        else if(date.Date.Ticks >= entity.Value.EstimatedDate.Date.Ticks)
        {
            var diff = date.Date - entity.Value.EstimatedDate;

            var cost = entity.Value.PostEndDateFine * diff.Days;

            entity.Value.Cost += cost;
        }

        entity.Value.EstimatedDate = date.Date;

        return await UpdateAsync(entity.Value, cancellationToken);
    }
}