
using LanguageExt.Common;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class RentFacade : IRentFacade
{
    private readonly IDataService<Entities.Rent> _dataService;

    private readonly IDataService<RentalPlane> _rentalPlaneDataService;

    private readonly IDataService<Vehicle> _vehicleService;

    public RentFacade(IDataService<Entities.Rent> dataService,
        IDataService<RentalPlane> rentalPlaneDataService,
        IDataService<Vehicle> vehicleService)
    {
        _dataService = dataService;
        _rentalPlaneDataService = rentalPlaneDataService;
        _vehicleService = vehicleService;
    }

    public async Task<Result<RentResponse>> CreateAsync(CreateRentEvent @event, CancellationToken cancellationToken = default)
    {
        var rentalPlanes = await _rentalPlaneDataService.GetAsync(x => x.Id == @event.RentPlaneId, cancellationToken);

        return rentalPlanes.Match(rentalPlanes => 
        {
            var vehicle = _vehicleService.GetAsync(x => !x.IsRented, cancellationToken).GetAwaiter().GetResult();

            var vehicleId = Guid.Empty;

            var entity = _dataService.CreateAsync(new Entities.Rent
            {
                NumberOfDays = rentalPlanes.NumberOfDays,
                DailyCost = rentalPlanes.DailyCost,
                VehicleId = vehicleId,
                UserId = @event.UserId,
                StartDate = DateTime.Now.Date.AddDays(1),
                PreEndDatePercentageFine = rentalPlanes.PreEndDatePercentageFine,
                PostEndDateFine = rentalPlanes.PostEndDateFine,
                EstimatedDate = DateTime.Now.Date.AddDays(1).AddDays(rentalPlanes.NumberOfDays),
                EndDate = DateTime.Now.Date.AddDays(1).AddDays(rentalPlanes.NumberOfDays),
                Cost = rentalPlanes.DailyCost * rentalPlanes.NumberOfDays
            }).GetAwaiter().GetResult();

            return new RentResponse();
        }, exception => new Result<RentResponse>(exception));

        
    }

}