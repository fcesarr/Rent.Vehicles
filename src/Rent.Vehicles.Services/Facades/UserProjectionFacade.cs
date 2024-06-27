using System.Linq.Expressions;

using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class UserProjectionFacade : IUserProjectionFacade
{
    private readonly IUserProjectionDataService _dataService;

    private readonly ILicenseImageService _licenseImageService;

    public UserProjectionFacade(IUserProjectionDataService dataService, ILicenseImageService licenseImageService)
    {
        _dataService = dataService;
        _licenseImageService = licenseImageService;
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        var licensePathResult =
            await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);

        if (!licensePathResult.IsSuccess)
        {
            return licensePathResult.Exception!;
        }

        return await CreateAsync(licensePathResult.Value!,
            @event,
            cancellationToken);
    }

    public Task<Result<UserResponse>> GetAsync(Expression<Func<UserProjection, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserResponse>> UpdateAsync(UpdateUserProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private async Task<Result<UserResponse>> CreateAsync(string licensePath,
        CreateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.CreateAsync(new UserProjection
        {
            Id = @event.Id,
            Name = @event.Name,
            Number = @event.Number,
            Birthday = @event.Birthday,
            LicenseNumber = @event.LicenseNumber,
            LicenseType = @event.LicenseType switch
            {
                LicenseType.B => Entities.Types.LicenseType.B,
                LicenseType.AB => Entities.Types.LicenseType.AB,
                LicenseType.A or _ => Entities.Types.LicenseType.A
            },
            LicensePath = licensePath
        }, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return new UserResponse
        {
            Id = entity.Value!.Id
            // Number = entity.Value.Number,
            // Name = entity.Value.Name,
            // LicenseNumber = entity.Value.LicenseNumber,
            // LicenseType = entity.Value.LicenseType,
            // LicensePath = entity.Value.LicensePath,
            // Birthday = entity.Value.Birthday,
            // Created = entity.Value.Created
        };
    }
}
