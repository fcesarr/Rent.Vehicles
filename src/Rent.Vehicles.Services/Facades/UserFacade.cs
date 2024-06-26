using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Facades;

public class UserFacade : IUserFacade
{
    private readonly ILogger<UserFacade> _logger;

    private readonly IBase64StringValidator _base64StringValidator;

    private readonly IUserDataService _dataService;

    private readonly ILicenseImageService _licenseImageService;

    public UserFacade(ILogger<UserFacade> logger,
        IBase64StringValidator validator,
        IUserDataService dataService,
        ILicenseImageService licenseImageService)
    {
        _logger = logger;
        _base64StringValidator = validator;
        _dataService = dataService;
        _licenseImageService = licenseImageService;
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserEvent @event, CancellationToken cancellationToken = default)
    {
        var result = await _base64StringValidator.ValidateAsync(@event.LicenseImage, cancellationToken);

        if(!result.IsValid)
            return result.Exception!;

        var licensePathResult = await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);

        if(!licensePathResult.IsSuccess)
            return licensePathResult.Exception!;

        return await TreatCreateUserEventToResponseAsync(licensePathResult.Value!,
                @event,
                cancellationToken);
    }

    private async Task<Result<UserResponse>> TreatCreateUserEventToResponseAsync(string licensePath,
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
            LicenseType = @event.LicenseType switch {
                Messages.Types.LicenseType.B => Entities.Types.LicenseType.B,
                Messages.Types.LicenseType.AB => Entities.Types.LicenseType.AB,
                Messages.Types.LicenseType.A or _ => Entities.Types.LicenseType.A,
            },
            LicensePath = licensePath
        }, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        return new UserResponse
        {
            Id = entity.Value!.Id,
            Number = entity.Value.Number,
            Name = entity.Value.Name,
            LicenseNumber = entity.Value.LicenseNumber,
            LicenseType = entity.Value.LicenseType,
            LicensePath = entity.Value.LicensePath,
            Birthday = entity.Value.Birthday,
            Created = entity.Value.Created
        };
    }

    public async Task<Result<UserResponse>> UpdateAsync(UpdateUserEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        return await TreatUpdateUserEventToResponseAsync(entity.Value!, @event, cancellationToken);
    }

    private async Task<Result<UserResponse>> TreatUpdateUserEventToResponseAsync(User entity,
        UpdateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        var result = await _base64StringValidator.ValidateAsync(@event.LicenseImage, cancellationToken);

        if(!result.IsValid)
            return result.Exception!;

        var licensePathResult = await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);
        
        if(!licensePathResult.IsSuccess)
            return licensePathResult.Exception!;

        entity.LicensePath = licensePathResult.Value!;

        var entityResult = await _dataService.UpdateAsync(entity, cancellationToken);

        if(!entityResult.IsSuccess)
            return entityResult.Exception!;

        return new UserResponse
        {
            Id = entityResult.Value!.Id,
            Number = entityResult.Value.Number,
            Name = entityResult.Value.Name,
            LicenseNumber = entityResult.Value.LicenseNumber,
            LicenseType = entityResult.Value.LicenseType,
            LicensePath = entityResult.Value.LicensePath,
            Birthday = entityResult.Value.Birthday,
            Created = entityResult.Value.Created
        };
    }

    public async Task<Result<UserResponse>> GetAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(predicate, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        return new UserResponse
        {
            Id = entity.Value!.Id,
            Number = entity.Value.Number,
            Name = entity.Value.Name,
            LicenseNumber = entity.Value.LicenseNumber,
            LicenseType = entity.Value.LicenseType,
            LicensePath = entity.Value.LicensePath,
            Birthday = entity.Value.Birthday,
            Created = entity.Value.Created
        };
    }
}