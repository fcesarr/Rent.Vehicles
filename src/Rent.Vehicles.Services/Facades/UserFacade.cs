using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;

namespace Rent.Vehicles.Services.Facades;

public class UserFacade : IUserFacade
{
    private readonly IUserDataService _dataService;

    private readonly ILicenseImageService _licenseImageService;
    private readonly ILogger<UserFacade> _logger;

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
        Result<string> licensePathResult =
            await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);

        if (!licensePathResult.IsSuccess)
        {
            return licensePathResult.Exception!;
        }

        return await TreatCreateUserEventToResponseAsync(licensePathResult.Value!,
            @event,
            cancellationToken);
    }

    public async Task<Result<UserResponse>> UpdateAsync(UpdateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        Result<User> entity = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return await TreatUpdateUserEventToResponseAsync(entity.Value!, @event, cancellationToken);
    }

    public async Task<Result<UserResponse>> GetAsync(Expression<Func<User, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        Result<User> entity = await _dataService.GetAsync(predicate, cancellationToken);

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

    private async Task<Result<UserResponse>> TreatCreateUserEventToResponseAsync(string licensePath,
        CreateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        Result<User> entity = await _dataService.CreateAsync(new User
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

    private async Task<Result<UserResponse>> TreatUpdateUserEventToResponseAsync(User entity,
        UpdateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        Result<string> licensePathResult =
            await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);

        if (!licensePathResult.IsSuccess)
        {
            return licensePathResult.Exception!;
        }

        entity.LicensePath = licensePathResult.Value!;

        Result<User> entityResult = await _dataService.UpdateAsync(entity, cancellationToken);

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