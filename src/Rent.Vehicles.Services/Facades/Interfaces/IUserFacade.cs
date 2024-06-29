using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IUserFacade : IFacade
{
    Task<Result<UserResponse>> CreateAsync(CreateUserEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> UpdateAsync(UpdateUserEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> UpdateAsync(UpdateUserLicenseImageEvent @event,
        CancellationToken cancellationToken = default);
}
