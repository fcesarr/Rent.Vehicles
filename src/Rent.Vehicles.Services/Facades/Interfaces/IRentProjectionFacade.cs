using System.Linq.Expressions;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IRentProjectionFacade : IFacade
{
    Task<Result<RentResponse>> CreateAsync(CreateRentProjectionEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<RentResponse>> UpdateAsync(UpdateRentProjectionEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<RentResponse>> GetAsync(Expression<Func<RentProjection, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<Result<CostResponse>> EstimateCostAsync(Guid id, DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<RentalPlaneResponse>>> FindAllRentalPlanesAsync(CancellationToken cancellationToken = default);
}
