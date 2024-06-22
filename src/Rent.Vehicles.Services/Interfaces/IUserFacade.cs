using LanguageExt.Common;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Dtos;

namespace Rent.Vehicles.Services.Interfaces;

public interface IUserFacade : IAction<User>
{
    Task<Result<User>> CreateAsync(UserDto dto, CancellationToken cancellationToken = default);

    Task<Result<User>> UpdateAsync(Guid id, string licenseImage, CancellationToken cancellationToken = default);
}