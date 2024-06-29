using System.Linq.Expressions;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IUserProjectionFacade : IFacade
{
    Task<Result<UserResponse>> CreateAsync(CreateUserProjectionEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> UpdateAsync(UpdateUserProjectionEvent @event,
        CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetAsync(Expression<Func<UserProjection, bool>> predicate,
        CancellationToken cancellationToken = default);
}
