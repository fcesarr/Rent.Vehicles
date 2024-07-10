using System.Linq.Expressions;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IVehicleProjectionFacade : IFacade
{
    Task<Result<VehicleResponse>> CreateAsync(CreateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<VehicleResponse>> UpdateAsync(UpdateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(DeleteVehiclesProjectionEvent @event, CancellationToken cancellationToken = default);

    Task<Result<VehicleResponse>> GetAsync(Expression<Func<VehicleProjection, bool>> predicate,
        CancellationToken cancellationToken = default);
}
