
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

    public async Task<LanguageExt.Common.Result<RentResponse>> CreateAsync(CreateRentEvent @event, CancellationToken cancellationToken = default)
    {
        var rentalPlanes = await _rentalPlaneDataService.GetAsync(x => x.Id == @event.RentPlaneId, cancellationToken);

        if(!rentalPlanes.IsSuccess)
            return new LanguageExt.Common.Result<RentResponse>(rentalPlanes.Exception);

        
        var vehicle = await _vehicleService.GetAsync(x => !x.IsRented, cancellationToken);

        if(!vehicle.IsSuccess)
            return new LanguageExt.Common.Result<RentResponse>(vehicle.Exception);

        var vehicleId = vehicle.Value;

        var entity = await _dataService.CreateAsync(new Entities.Rent
        {
            NumberOfDays = rentalPlanes.Value.NumberOfDays,
            DailyCost = rentalPlanes.Value.DailyCost,
            VehicleId = vehicleId.Id,
            UserId = @event.UserId,
            StartDate = DateTime.Now.Date.AddDays(1),
            PreEndDatePercentageFine = rentalPlanes.Value.PreEndDatePercentageFine,
            PostEndDateFine = rentalPlanes.Value.PostEndDateFine,
            EstimatedDate = DateTime.Now.Date.AddDays(1).AddDays(rentalPlanes.Value.NumberOfDays),
            EndDate = DateTime.Now.Date.AddDays(1).AddDays(rentalPlanes.Value.NumberOfDays),
            Cost = rentalPlanes.Value.DailyCost * rentalPlanes.Value.NumberOfDays
        });

        return new RentResponse();
    }

}