using System.Linq.Expressions;

using LanguageExt.Common;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IUserFacade
{
    Task<Result<UserResponse>> CreateAsync(CreateUserEvent @event, CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> UpdateAsync(UpdateUserEvent @event, CancellationToken cancellationToken = default);

    Task<Result<UserResponse>> GetAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default);
}