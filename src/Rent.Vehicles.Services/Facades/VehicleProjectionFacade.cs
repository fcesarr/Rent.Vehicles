using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class VehicleProjectionFacade : IVehicleProjectionFacade
{
    private readonly IDataService<VehicleProjection> _dataService;
    private readonly IVehicleDataService _vehicleDataService;

    public VehicleProjectionFacade(IVehicleDataService vehicleDataService, IDataService<VehicleProjection> dataService)
    {
        _vehicleDataService = vehicleDataService;
        _dataService = dataService;
    }

    public async Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicleDataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!vehicle.IsSuccess)
        {
            return vehicle.Exception!;
        }

        var entity =
            await _dataService.CreateAsync(vehicle.Value!.ToProjection<VehicleProjection>(), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }

    public async Task<Result<bool>> DeleteAsync(DeleteVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.DeleteAsync(@event.Id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity;
    }

    public async Task<Result<VehicleResponse>> UpdateAsync(UpdateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicleDataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!vehicle.IsSuccess)
        {
            return vehicle.Exception!;
        }

        var entity =
            await _dataService.UpdateAsync(vehicle.Value!.ToProjection<VehicleProjection>(), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }
}
