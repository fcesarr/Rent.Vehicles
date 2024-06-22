using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MongoDB.Bson;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Dtos;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Settings;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class UserFacade : IUserFacade
{
    private readonly ILogger<UserFacade> _logger;

    private readonly IBase64StringValidator _base64StringValidator;

    private readonly IUserService _userService;

    private readonly ILicenseImageService _licenseImageService;

    public UserFacade(ILogger<UserFacade> logger,
        IBase64StringValidator validator,
        IUserService userService,
        ILicenseImageService licenseImageService)
    {
        _logger = logger;
        _base64StringValidator = validator;
        _userService = userService;
        _licenseImageService = licenseImageService;
    }

    public async Task<Result<User>> CreateAsync(UserDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _base64StringValidator.ValidateAsync(dto.LicenseImage, cancellationToken);

        if(!result.IsValid)
            return new Result<User>(result.Exception);

        var licensePathResult = await _licenseImageService.GetPathAsync(dto.LicenseImage, cancellationToken);

        return await licensePathResult
            .Match(async licensePath => await _userService.CreateAsync(new User
            {
                Id = dto.Id,
                Name = dto.Name,
                Number = dto.Number,
                Birthday = dto.Birthday,
                LicenseNumber = dto.LicenseNumber,
                LicenseType = dto.LicenseType,
                LicensePath = licensePath
            }, cancellationToken), exception => Task.FromResult(new Result<User>(exception)));
    }

    public async Task<Result<User>> UpdateAsync(Guid id, string licenseImage, CancellationToken cancellationToken = default)
    {
        var entity = await _userService.GetAsync(x => x.Id == id, cancellationToken);

        return await entity.Match(async entity => 
        {
            var result = await _base64StringValidator.ValidateAsync(licenseImage, cancellationToken);

            if(!result.IsValid)
                return new Result<User>(result.Exception);

            var licensePathResult = await _licenseImageService.GetPathAsync(licenseImage, cancellationToken);
            
            return await licensePathResult.Match(async licensePath => {
                entity.LicensePath = licensePath;

                return await _userService.UpdateAsync(entity, cancellationToken);
            }, exception => Task.FromResult(new Result<User>(exception)));

        }, exception => Task.FromResult(new Result<User>(exception)));
    }
}