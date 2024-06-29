using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IRentFacade : IFacade
{
    Task<Result<RentResponse>> CreateAsync(CreateRentEvent @event, CancellationToken cancellationToken = default);

    Task<Result<RentResponse>> UpdateAsync(UpdateRentEvent @event, CancellationToken cancellationToken = default);
}
