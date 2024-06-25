
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IRentFacade
{
    Task<LanguageExt.Common.Result<RentResponse>> CreateAsync(CreateRentEvent @event, CancellationToken cancellationToken = default);
}