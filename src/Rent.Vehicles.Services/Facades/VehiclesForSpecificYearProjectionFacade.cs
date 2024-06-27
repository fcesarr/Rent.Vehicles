
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class VehiclesForSpecificYearProjectionFacade : IVehiclesForSpecificYearProjectionFacade
{
    private readonly IVehicleDataService _vehicleDataService;

    private readonly IDataService<VehiclesForSpecificYearProjection> _dataService;

    public VehiclesForSpecificYearProjectionFacade(IVehicleDataService vehicleDataService, IDataService<VehiclesForSpecificYearProjection> dataService)
    {
        _vehicleDataService = vehicleDataService;
        _dataService = dataService;
    }

    public async Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesForSpecificYearProjectionEvent @event, CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicleDataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if(!vehicle.IsSuccess)
        {
            return vehicle.Exception!;
        }

        var entity = await _dataService.CreateAsync(vehicle.Value!.ToProjection<VehiclesForSpecificYearProjection>(), cancellationToken);

        if(!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }
}
