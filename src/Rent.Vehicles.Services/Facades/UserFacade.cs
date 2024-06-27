using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class UserFacade : IUserFacade
{
    private readonly IUserDataService _dataService;
    private readonly ILicenseImageService _licenseImageService;
    private readonly ILogger<UserFacade> _logger;
    private readonly IUserProjectionDataService _projectionDataService;

    public UserFacade(ILogger<UserFacade> logger,
        IUserDataService dataService,
        ILicenseImageService licenseImageService)
    {
        _logger = logger;
        _dataService = dataService;
        _licenseImageService = licenseImageService;
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserEvent @event,
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

    public async Task<Result<UserResponse>> UpdateAsync(UpdateUserLicenseImageEvent @event,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return await UpdateAsync(entity.Value!, @event, cancellationToken);
    }

    public Task<Result<UserResponse>> UpdateAsync(UpdateUserEvent @event, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private async Task<Result<UserResponse>> CreateAsync(string licensePath,
        CreateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.CreateAsync(new User
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

    private async Task<Result<UserResponse>> UpdateAsync(User entity,
        UpdateUserLicenseImageEvent @event,
        CancellationToken cancellationToken = default)
    {
        var licensePathResult =
            await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);

        if (!licensePathResult.IsSuccess)
        {
            return licensePathResult.Exception!;
        }

        entity.LicensePath = licensePathResult.Value!;

        var entityResult = await _dataService.UpdateAsync(entity, cancellationToken);

        if (!entityResult.IsSuccess)
        {
            return entityResult.Exception!;
        }

        return new UserResponse
        {
            Id = entityResult.Value!.Id
            // Number = entityResult.Value.Number,
            // Name = entityResult.Value.Name,
            // LicenseNumber = entityResult.Value.LicenseNumber,
            // LicenseType = entityResult.Value.LicenseType,
            // LicensePath = entityResult.Value.LicensePath,
            // Birthday = entityResult.Value.Birthday,
            // Created = entityResult.Value.Created
        };
    }
}
