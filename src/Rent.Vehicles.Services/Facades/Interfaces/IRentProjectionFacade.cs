using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IRentProjectionFacade : IFacade
{
    Task<Result<RentResponse>> CreateAsync(CreateRentProjectionEvent @event, CancellationToken cancellationToken = default);

    Task<Result<RentResponse>> UpdateAsync(UpdateRentProjectionEvent @event, CancellationToken cancellationToken = default);

    Task<Result<CostResponse>> EstimateCostAsync(Guid id, DateTime endDate,
        CancellationToken cancellationToken = default);
}
