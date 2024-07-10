using System.Linq.Expressions;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class VehicleProjectionFacade : IVehicleProjectionFacade
{
    private readonly IVehicleDataService _dataService;
    private readonly IVehicleProjectionDataService _projectionDataService;

    public VehicleProjectionFacade(IVehicleDataService vehicleDataService, IVehicleProjectionDataService dataService)
    {
        _dataService = vehicleDataService;
        _projectionDataService = dataService;
    }

    public async Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!vehicle.IsSuccess)
        {
            return vehicle.Exception!;
        }

        var entity =
            await _projectionDataService.CreateAsync(vehicle.Value!.ToProjection<VehicleProjection>(),
                cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }

    public async Task<Result<bool>> DeleteAsync(DeleteVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var entity = await _projectionDataService.DeleteAsync(@event.Id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity;
    }

    public async Task<Result<VehicleResponse>> GetAsync(Expression<Func<VehicleProjection, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entity = await _projectionDataService.GetAsync(predicate, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }

    public async Task<Result<VehicleResponse>> UpdateAsync(UpdateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!vehicle.IsSuccess)
        {
            return vehicle.Exception!;
        }

        var entity =
            await _projectionDataService.UpdateAsync(vehicle.Value!.ToProjection<VehicleProjection>(),
                cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return entity.Value!.ToResponse();
    }
}
