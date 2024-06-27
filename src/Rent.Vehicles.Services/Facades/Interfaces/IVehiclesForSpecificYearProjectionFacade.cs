using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IVehiclesForSpecificYearProjectionFacade
{
    Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesForSpecificYearProjectionEvent @event,
        CancellationToken cancellationToken = default);
}
