using LanguageExt.Common;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Events;

namespace Rent.Vehicles.Services.Facades.Interfaces;

public interface IUserFacade
{
    Task<Result<User>> CreateAsync(CreateUserEvent @event, CancellationToken cancellationToken = default);
}