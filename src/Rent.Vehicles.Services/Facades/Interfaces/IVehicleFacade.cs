using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IVehicleFacade : IFacade
{
    Task<Result<VehicleResponse>>
        CreateAsync(CreateVehiclesEvent @event, CancellationToken cancellationToken = default);

    Task<Result<VehicleResponse>>
        UpdateAsync(UpdateVehiclesEvent @event, CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(DeleteVehiclesEvent @event, CancellationToken cancellationToken = default);
}
