using LanguageExt.Common;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Dtos;

namespace Rent.Vehicles.Services.Interfaces;

public interface IUserFacade : IFacade<UserDto, User>
{
    Task<Result<User>> UpdateAsync(Guid id, string licenseImage, CancellationToken cancellationToken = default);
}