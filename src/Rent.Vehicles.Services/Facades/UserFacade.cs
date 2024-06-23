using LanguageExt.Common;

using Microsoft.Extensions.Logging;

using MongoDB.Bson;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
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

    public async Task<Result<User>> CreateAsync(CreateUserEvent @event, CancellationToken cancellationToken = default)
    {
        var result = await _base64StringValidator.ValidateAsync(@event.LicenseImage, cancellationToken);

        if(!result.IsValid)
            return new Result<User>(result.Exception);

        var licensePathResult = await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);

        return await licensePathResult
            .Match(async licensePath => await _dataService.CreateAsync(new User
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
            }, cancellationToken), exception => Task.FromResult(new Result<User>(exception)));
    }

    public async Task<Result<User>> UpdateAsync(UpdateUserEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _dataService.GetAsync(x => x.Id == @event.Id, cancellationToken);

        return await entity.Match(async entity => 
        {
            var result = await _base64StringValidator.ValidateAsync(@event.LicenseImage, cancellationToken);

            if(!result.IsValid)
                return new Result<User>(result.Exception);

            var licensePathResult = await _licenseImageService.GetPathAsync(@event.LicenseImage, cancellationToken);
            
            return await licensePathResult.Match(async licensePath => {
                entity.LicensePath = licensePath;

                return await _dataService.UpdateAsync(entity, cancellationToken);
            }, exception => Task.FromResult(new Result<User>(exception)));

        }, exception => Task.FromResult(new Result<User>(exception)));
    }
}