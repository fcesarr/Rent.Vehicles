using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IVehicleProjectionFacade
{
    Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<VehicleResponse>> UpdateAsync(UpdateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(DeleteVehiclesProjectionEvent @event, CancellationToken cancellationToken = default);
}
