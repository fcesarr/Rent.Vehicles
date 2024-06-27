using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IVehicleProjectionFacade
{
    Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesProjectionEvent @event, CancellationToken cancellationToken = default);
}
