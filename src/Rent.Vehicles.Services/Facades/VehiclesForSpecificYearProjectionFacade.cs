using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class VehiclesForSpecificYearProjectionFacade : IVehiclesForSpecificYearProjectionFacade
{
    private readonly IVehiclesForSpecificYearProjectionDataService _dataProjectionService;
    private readonly IVehicleDataService _dataService;

    public VehiclesForSpecificYearProjectionFacade(IVehiclesForSpecificYearProjectionDataService dataProjectionService, IVehicleDataService dataService)
    {
        _dataProjectionService = dataProjectionService;
        _dataService = dataService;
    }

    public async Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesForSpecificYearProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!vehicle.IsSuccess)
        {
            return vehicle.Exception!;
        }

        var entity = await _dataProjectionService.CreateAsync(vehicle.Value!.ToProjection<VehiclesForSpecificYearProjection>(),
            cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }
}
