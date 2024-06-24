using System.Linq.Expressions;

using LanguageExt;
using LanguageExt.Common;

using Microsoft.Extensions.Logging;

using MongoDB.Bson;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Responses;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

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
            return new Result<UserResponse>(result.Exception);

        var licensePathResult = await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);

        return await licensePathResult
            .Match(async licensePath => await TreatCreateUserEventToResponseAsync(licensePath,
                @event,
                cancellationToken), exception => Task.FromResult(new Result<UserResponse>(exception)));
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

        return entity.Match(entity => new UserResponse
        {
            Id = entity.Id,
            Number = entity.Number,
            Name = entity.Name,
            LicenseNumber = entity.LicenseNumber,
            LicenseType = entity.LicenseType,
            LicensePath = entity.LicensePath,
            Birthday = entity.Birthday,
            Created = entity.Created
        }, exception => new Result<UserResponse>(exception));
    }

    public async Task<Result<UserResponse>> UpdateAsync(UpdateUserEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        return await entity.Match(async entity => await TreatUpdateUserEventToResponseAsync(entity, @event, cancellationToken),
            exception => Task.FromResult(new Result<UserResponse>(exception)));
    }

    private async Task<Result<UserResponse>> TreatUpdateUserEventToResponseAsync(User entity,
        UpdateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        var result = await _base64StringValidator.ValidateAsync(@event.LicenseImage, cancellationToken);

        if(!result.IsValid)
            return new Result<UserResponse>(result.Exception);

        var licensePathResult = await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);
        
        return await licensePathResult.Match(async licensePath => {
            entity.LicensePath = licensePath;

            var entityResult = await _dataService.UpdateAsync(entity, cancellationToken);

            return entityResult.Match(entity => new UserResponse
                {
                    Id = entity.Id,
                    Number = entity.Number,
                    Name = entity.Name,
                    LicenseNumber = entity.LicenseNumber,
                    LicenseType = entity.LicenseType,
                    LicensePath = entity.LicensePath,
                    Birthday = entity.Birthday,
                    Created = entity.Created
                },
                exception => new Result<UserResponse>(exception));
        }, exception => Task.FromResult(new Result<UserResponse>(exception)));
    }

    public async Task<Result<UserResponse>> GetAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(predicate, cancellationToken);

        return entity.Match(entity => new UserResponse
        {
            Id = entity.Id,
            Number = entity.Number,
            Name = entity.Name,
            LicenseNumber = entity.LicenseNumber,
            LicenseType = entity.LicenseType,
            LicensePath = entity.LicensePath,
            Birthday = entity.Birthday,
            Created = entity.Created
        }, exception => new Result<UserResponse>(exception));
    }
}